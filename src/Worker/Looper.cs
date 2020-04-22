using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Worker
{
    internal interface ILooper
    {
        bool Running { get; set; }
        void DoActualLoopWork();
    }

    internal class Looper : ILooper
    {
        private readonly IPickANumber _pickANumber;
        private readonly Metric _metric;
        private readonly ILogger _logger;

        public Looper(ILoggerFactory loggerFactory, IPickANumber pickANumber, TelemetryClient telemetryClient)
        {
            _pickANumber = pickANumber;
            _metric = telemetryClient.GetMetric($"{nameof(Looper)}.RunDuration");
            _logger = loggerFactory.CreateLogger<Looper>();
        }

        public bool Running { get; set; }

        public void DoActualLoopWork()
        {
            _logger.LogInformation($"{nameof(DoActualLoopWork)} started.");
            var start = DateTime.UtcNow;
            do
            {
                Thread.Sleep(1000);
                _logger.LogDebug( $"{_pickANumber.Random()}");
            } while (Running);

            var end = DateTime.UtcNow;
            _metric.TrackValue((end-start).TotalMilliseconds);
            _logger.LogInformation($"{nameof(DoActualLoopWork)} ended.");
            Thread.Sleep(1000);
        }
    }
}