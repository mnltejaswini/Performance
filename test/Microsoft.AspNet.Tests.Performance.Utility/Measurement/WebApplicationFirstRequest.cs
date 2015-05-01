// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Framework.Logging;

namespace Microsoft.AspNet.Tests.Performance.Utility.Measurement
{
    public class WebApplicationFirstRequest
    {
        //private readonly ProcessStartInfo _processInfo;
        //private readonly int _port;
        //private readonly string _path;

        private readonly StartupRunnerOptions _options;
        private readonly string _url;
        private ILogger _logger;
        private readonly int _retry = 10;
        private readonly TimeSpan _timeout; // in seconds

        public WebApplicationFirstRequest(StartupRunnerOptions options,
                                          TimeSpan timeout,
                                          int port,
                                          string path)
        {
            _options = options;

            _url = string.Format("http://localhost:{0}{1}", port, path);
            _logger = _options.Logger;
            _timeout = timeout;
        }

        public bool Run()
        {
            var client = new HttpClient();

            _logger.LogData("Measurer", typeof(WebApplicationFirstRequest).Name, infoOnly: true);
            _logger.LogData("CommandFilename", _options.ProcessStartInfo.FileName, infoOnly: true);
            _logger.LogData("CommandArguments", _options.ProcessStartInfo.Arguments, infoOnly: true);
            _logger.LogData("Url", _url, infoOnly: true);
            _logger.LogData("Timeout", _timeout, infoOnly: true);

            var repeater = new Repeater<RunResult>(
                body: (iteration, result) =>
                {
                    Task<HttpResponseMessage> webtask = null;
                    bool responseRetrived = false;

                    var sw = new Stopwatch();
                    sw.Start();

                    var process = Process.Start(_options.ProcessStartInfo);

                    for (int i = 0; i < _retry; ++i)
                    {
                        try
                        {
                            _logger.LogInformation("Try {0}: GET {1}. [Iteration {2}]", i, _url, iteration);
                            webtask = client.GetAsync(_url);

                            if (webtask.Wait(_timeout))
                            {
                                responseRetrived = true;
                                break;
                            }
                            else
                            {
                                _logger.LogError("Http client timeout. [Iteration {0}]", iteration);
                                break;
                            }
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }

                    sw.Stop();


                    if (process != null && !process.HasExited)
                    {
                        _logger.LogInformation("Kill proces {0}", process.Id);
                        process.Kill();
                    }

                    result.Elapsed = sw.ElapsedMilliseconds;

                    if (responseRetrived)
                    {
                        var response = webtask.Result;
                        result.StatusCode = response.StatusCode;
                        result.ResponseHead = response.ToString();

                        if (!response.IsSuccessStatusCode)
                        {
                            _logger.LogError(string.Format("Request failed. {0}", response.StatusCode));
                            result.Success = false;
                        }

                        result.Success = true;
                    }
                    else
                    {
                        result.Success = false;
                    }
                },
                exceptionHandler: (ex, iteration, result) =>
                {
                    result.Success = false;
                    result.Exception = ex;
                });

            var results = repeater.Execute(_options.IterationCount);
            var successful = results.Where(r => r.Success);

            _logger.LogData("Successful rate", successful.Count() / results.Count(), infoOnly: true);
            _logger.LogData("Successful iteration", successful.Count(), infoOnly: true);
            _logger.LogData("Time", successful.Average(r => r.Elapsed));

            return true;
        }

        private class RunResult
        {
            public HttpStatusCode StatusCode { get; set; }

            public string ResponseHead { get; set; }

            public long Elapsed { get; set; }

            public Exception Exception { get; set; }

            public bool Success { get; set; }
        }
    }
}