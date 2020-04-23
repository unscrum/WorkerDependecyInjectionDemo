using Microsoft.ApplicationInsights;

namespace Worker.Services
{
    public interface IMyMetric
    {
        void TrackValue(double v);
    }
    public interface IMyTelemetry
    {
        IMyMetric GetMetric(string name);
    }
    class MyTelemetry:IMyTelemetry
    {
        private readonly TelemetryClient _telemetryClient;

        public MyTelemetry(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }


        public IMyMetric GetMetric(string name)
        {
            return new MyMetric(_telemetryClient.GetMetric(name));
        }
    }

    class MyMetric:IMyMetric
    {
        internal Metric Metric { get; }

        public MyMetric(Metric metric)
        {
            Metric = metric;
        }

        public void TrackValue(double v)
        {
            Metric.TrackValue(v);
        }
        
        
    }
}