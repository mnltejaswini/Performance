// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Benchmarks.Framework;
using Microsoft.Extensions.Logging;

namespace Benchmarks.Utility.Measurement
{
    public class WebHostFirstRequest
    {
        private readonly StartupRunnerOptions _options;
        private readonly string _url;
        private ILogger _logger;
        private readonly int _retry = 10;
        private readonly TimeSpan _timeout; // in seconds

        public WebHostFirstRequest(StartupRunnerOptions options,
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

            _logger.LogInformation($"Measurer: {typeof(WebHostFirstRequest).Name}");
            _logger.LogInformation($"CommandFilename: { _options.ProcessStartInfo.FileName}");
            _logger.LogInformation($"CommandArguments: {_options.ProcessStartInfo.Arguments}");
            _logger.LogInformation($"Url: {_url}");
            _logger.LogInformation($"Timeout: {_timeout}");

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

            foreach (var one in successful)
            {
                _options.Summary.Aggregate(new BenchmarkIterationSummary
                {
                    TimeElapsed = (long)one.Elapsed
                });
            }
            _options.Summary.PopulateMetrics();

            _logger.LogInformation(_options.Summary.ToString());

            return true;
        }

        private class RunResult
        {
            public HttpStatusCode StatusCode { get; set; }

            public string ResponseHead { get; set; }

            public double Elapsed { get; set; }

            public Exception Exception { get; set; }

            public bool Success { get; set; }
        }
    }
}