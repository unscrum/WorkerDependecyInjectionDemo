using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.ApplicationInsights;

namespace Worker.Services
{
    internal interface IPickANumber
    {
        int Random();
    }

    internal class PickANumber : IPickANumber
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly IDictionary<int, Metric> _dictionary;
        public PickANumber(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
            _dictionary = new Dictionary<int, Metric>();
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