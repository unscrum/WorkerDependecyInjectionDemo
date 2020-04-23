using System.Collections.Generic;
using System.Security.Cryptography;

namespace Worker.Services
{
    public interface IPickANumber
    {
        int Random();
    }

    internal class PickANumber : IPickANumber
    {
        private readonly IMyTelemetry _telemetryClient;
        private readonly IDictionary<int, IMyMetric> _dictionary;
        public PickANumber(IMyTelemetry telemetryClient)
        {
            _telemetryClient = telemetryClient;
            _dictionary = new Dictionary<int, IMyMetric>();
        }

        private void Increment(int number)
        {
            if (!_dictionary.ContainsKey(number))
                _dictionary.Add(number, _telemetryClient.GetMetric($"{nameof(Random)}-{number}"));

            _dictionary[number].TrackValue(1);

        }
        public int Random()
        {
            var number =  RandomNumberGenerator.GetInt32(11);
            Increment(number);
            return number;
        }
    }
}