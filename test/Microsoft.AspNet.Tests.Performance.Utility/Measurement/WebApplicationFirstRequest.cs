// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Framework.Logging;

namespace Microsoft.AspNet.Tests.Performance.Utility.Measurement
{
    public class WebApplicationFirstRequest
    {
        private readonly ProcessStartInfo _processInfo;
        private readonly int _timeout; // in seconds
        private readonly int _port;
        private readonly string _path;

        private readonly EventWaitHandle _waitWebListenerReady;
        private ILogger _logger;

        public WebApplicationFirstRequest(ProcessStartInfo processInfo,
                                          ILogger logger,
                                          int timeout = 60,
                                          int port = -1,
                                          string path = "/")
        {
            _processInfo = processInfo;
            _timeout = timeout;
            _port = port;
            _path = path;

            _waitWebListenerReady = new AutoResetEvent(false);

            _logger = logger;
        }

        public bool Run()
        {
            _logger.LogData("Measurer", typeof(WebApplicationFirstRequest).Name, infoOnly: true);

            var client = new HttpClient();
            var url = string.Format("http://localhost:{0}{1}", _port, _path);

            Task<HttpResponseMessage> webtask = null;
            bool failure = false;

            _processInfo.RedirectStandardOutput = true;
            _processInfo.RedirectStandardError = true;
            _processInfo.UseShellExecute = false;

            _logger.LogData("CommandFilename", _processInfo.FileName, infoOnly: true);
            _logger.LogData("CommandArguments", _processInfo.Arguments, infoOnly: true);

            var sw = new Stopwatch();
            sw.Start();

            var process = Process.Start(_processInfo);
            process.OutputDataReceived += Process_OutputDataReceived;
            process.BeginOutputReadLine();

            var timeout = !_waitWebListenerReady.WaitOne(_timeout * 1000);

            if (timeout)
            {
                _logger.LogError("Web listener timeout during starting.");
                return false;
            }

            try
            {
                webtask = client.GetAsync(url);
                timeout = !webtask.Wait(_timeout * 1000);

                if (timeout)
                {
                    _logger.LogError("Test case timeout");
                    failure = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Test failed for unexpected exception: " + ex.Message + "\n" + ex.StackTrace);
                failure = true;
            }
            finally
            {
                if (process != null && !process.HasExited)
                {
                    Console.WriteLine("Kill proces {0}", process.Id);
                    process.Kill();
                }
            }

            sw.Stop();

            if (failure)
            {
                return false;
            }

            var response = webtask.Result;
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(string.Format("Request failed. {0}", response.StatusCode));
                return false;
            }

            _logger.LogData("StatusCode", response.StatusCode, infoOnly: true);
            _logger.LogData("ResponseHead", response.ToString(), infoOnly: true);
            _logger.LogData("Time", sw.ElapsedMilliseconds);

            return true;
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == "Started")
            {
                _waitWebListenerReady.Set();
            }
        }
    }
}