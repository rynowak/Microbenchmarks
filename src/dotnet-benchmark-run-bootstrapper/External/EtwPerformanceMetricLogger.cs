// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Diagnostics.Tracing;
using Microsoft.Xunit.Performance.Sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.Xunit.Performance
{
    internal class EtwPerformanceMetricLogger : IPerformanceMetricLogger
    {
        private readonly string _etlPath;
        private readonly XunitPerformanceProject _project;
        private readonly List<PerformanceTestInfo> _tests = new List<PerformanceTestInfo>();

        public EtwPerformanceMetricLogger(XunitPerformanceProject project)
        {
            _etlPath = Path.Combine(project.OutputDir, project.OutputBaseFileName + ".etl");
            _project = project;
        }

        public IDisposable StartLogging(System.Diagnostics.ProcessStartInfo runnerStartInfo)
        {
            Logger.PrintIfVerbose($"Starting ETW tracing. Logging to {_etlPath}");

            var allEtwProviders =
                from test in _tests
                from metric in test.Metrics.Cast<PerformanceMetric>()
                from provider in metric.ProviderInfo
                select provider;

            var mergedEtwProviders = ProviderInfo.Merge(allEtwProviders);

            return ETWLogging.StartAsync(_etlPath, mergedEtwProviders).GetAwaiter().GetResult();
        }

        public IPerformanceMetricReader GetReader()
        {
            using (var source = new ETWTraceEventSource(_etlPath))
            {
                if (source.EventsLost > 0)
                    throw new Exception($"Events were lost in trace '{_etlPath}'");

                var evaluationContext = new EtwPerformanceMetricEvaluationContext(_etlPath, source, _tests, _project.RunId);
                try
                {
                    source.Process();
                    return evaluationContext;
                }
                catch
                {
                    evaluationContext.Dispose();
                    throw;
                }
            }
        }

        private class ConsoleReporter : IMessageSink
        {
            public bool OnMessage(IMessageSinkMessage message)
            {
                Console.WriteLine(message.ToString());
                return true;
            }
        }
    }
}
