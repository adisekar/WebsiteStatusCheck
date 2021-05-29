using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebsiteStatus
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private IHttpClientFactory _clientFactory { get; set; }

        public Worker(ILogger<Worker> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("The service has been started...");
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("The service has been stopped...");
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var client = _clientFactory.CreateClient();
                var result = await client.GetAsync("http://adisekar.com");
                if (result.IsSuccessStatusCode)
                {
                    _logger.LogInformation("The website is up. Status code: {statusCode}", result.StatusCode);
                }
                else
                {
                    // Fire an email if website is down.
                    _logger.LogError("The website is down. Status code: {statusCode}", result.StatusCode);
                }

                // Run every 5 minutes
                await Task.Delay(5 * 60 * 1000, stoppingToken);
            }
        }
    }
}
