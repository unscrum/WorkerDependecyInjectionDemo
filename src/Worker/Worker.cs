using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Worker
{
    internal class Worker : IHostedService
    {
        private readonly ILogger _logger;
        private readonly ILooper _looper;

        public Worker(ILoggerFactory loggerFactory, ILooper looper)
        {
            _looper = looper;
            _logger = loggerFactory.CreateLogger<Worker>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Worker is starting up.");
            _looper.Running = true;
            Task.Factory.StartNew(_looper.DoActualLoopWork, cancellationToken);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Worker is stopping.");
            _looper.Running = false;
            Thread.Sleep(1000);
            return Task.CompletedTask;
        }
    }
}