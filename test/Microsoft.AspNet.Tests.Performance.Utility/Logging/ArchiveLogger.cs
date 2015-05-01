// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Framework.Logging;
using Newtonsoft.Json;

namespace Microsoft.AspNet.Tests.Performance.Utility.Logging
{
    public class ArchiveLogger : ILogger
    {
        private readonly string _artifactsFolder;
        private readonly string _name;

        private LogFileHandler _logfileHandler;

        public ArchiveLogger(string name, string artifactsFolder)
        {
            _name = name;
            _artifactsFolder = artifactsFolder;
        }

        public IDisposable BeginScopeImpl(object state)
        {
            if (_logfileHandler == null || _logfileHandler.Disposed)
            {
                var folder = Path.Combine(_artifactsFolder, _name);
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                _logfileHandler = new LogFileHandler(Path.Combine(folder, "result.json"));
            }

            return _logfileHandler;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            if (_logfileHandler == null)
            {
                return;
            }

            var data = LoggerHelper.RetrivePerformanceData(state);
            if (data != null)
            {
                _logfileHandler.AddData(data.Item1, data.Item2);
            }

            var info = LoggerHelper.RetrivePerformanceInfo(state);
            if (info != null)
            {
                _logfileHandler.AddInfo(info.Item1, info.Item2);
            }
        }

        private class LogFileHandler : IDisposable
        {
            private readonly Dictionary<string, string> _data;
            private readonly Dictionary<string, string> _info;
            private readonly string _filepath;

            public LogFileHandler(string filepath)
            {
                _data = new Dictionary<string, string>();
                _info = new Dictionary<string, string>();
                _filepath = filepath;
            }

            public bool Disposed { get; private set; }

            public void Dispose()
            {
                var content = JsonConvert.SerializeObject(new { Info = _info, Data = _data }, Formatting.Indented);
                File.WriteAllText(_filepath, content);

                Disposed = true;
            }

            public void AddData(string name, string value)
            {
                _data.Add(name, value);
            }

            public void AddInfo(string name, string value)
            {
                _info.Add(name, value);
            }
        }
    }
}