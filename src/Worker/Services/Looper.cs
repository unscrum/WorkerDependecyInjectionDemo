using System;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Worker.Services
{
    public interface ILooper
    {
        bool Running { get; set; }
        void DoActualLoopWork();
    }

    internal class Looper : ILooper
    {
        private readonly IPickANumber _pickANumber;
        private readonly IMyMetric _metric;
        private readonly ILogger _logger;

        public Looper(ILoggerFactory loggerFactory, IPickANumber pickANumber, IMyTelemetry telemetryClient)
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