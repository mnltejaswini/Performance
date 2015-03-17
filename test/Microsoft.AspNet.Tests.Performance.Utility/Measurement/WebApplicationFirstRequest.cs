// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Net.Http;
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

            _logger = logger;
        }

        public bool Run()
        {
            var client = new HttpClient();
            var url = string.Format("http://localhost:{0}{1}", _port, _path);

            _logger.LogData("Measurer", typeof(WebApplicationFirstRequest).Name, infoOnly: true);
            _logger.LogData("CommandFilename", _processInfo.FileName, infoOnly: true);
            _logger.LogData("CommandArguments", _processInfo.Arguments, infoOnly: true);
            _logger.LogData("Url", url, infoOnly: true);

            const int retry = 10;
            Task<HttpResponseMessage> webtask = null;
            bool responseRetrived = false;

            var sw = new Stopwatch();
            sw.Start();

            var process = Process.Start(_processInfo);

            for (int i = 0; i < retry; ++i)
            {
                try
                {
                    _logger.LogInformation("Try {0}: GET {1}", i, url);
                    webtask = client.GetAsync(url);

                    if (webtask.Wait(_timeout * 1000))
                    {
                        responseRetrived = true;
                        break;
                    }
                    else
                    {
                        _logger.LogError("Http client timeout");
                        break;
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }

            sw.Stop();

            _logger.LogInformation("Response retrieved.");

            if (process != null && !process.HasExited)
            {
                _logger.LogInformation("Kill proces {0}", process.Id);
                process.Kill();
            }

            _logger.LogData("Time", sw.ElapsedMilliseconds);

            if (responseRetrived)
            {
                var response = webtask.Result;
                _logger.LogData("StatusCode", response.StatusCode, infoOnly: true);
                _logger.LogData("ResponseHead", response.ToString(), infoOnly: true);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError(string.Format("Request failed. {0}", response.StatusCode));
                    return false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}