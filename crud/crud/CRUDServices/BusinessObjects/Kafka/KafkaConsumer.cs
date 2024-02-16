using Confluent.Kafka;
using System.Text.Json;
using static Confluent.Kafka.ConfigPropertyNames;
using CRUDServices.Models.Kafka;

namespace CollectionServices.BusinessObject.Kafka
{
    /// <summary>
    ///     A simple example demonstrating how to set up a Kafka consumer as an
    ///     IHostedService.
    /// </summary>
    public class KafkaConsumer : BackgroundService
    {
        private readonly string _closingEdwAktuwTopic;
        private readonly IConsumer<string, string> _EdwServiceConsumer;
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public KafkaConsumer(IConfiguration config, ILoggerFactory loggerFactory, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = loggerFactory.CreateLogger<KafkaConsumer>();
            _serviceScopeFactory = serviceScopeFactory;
            //_bOProduct = bOProduct;

            var consumerConfig = new ConsumerConfig();
            config.GetSection("Kafka:ConsumerSettings").Bind(consumerConfig);

            _closingEdwAktuwTopic = config.GetValue<string>("Kafka:namatopic");
           
            _EdwServiceConsumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
       
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(() => namaFunction(stoppingToken));
            return Task.CompletedTask;
        }
        private async Task namaFunction(CancellationToken cancellationToken)
        {
            _EdwServiceConsumer.Subscribe(_closingEdwAktuwTopic);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var cr = _EdwServiceConsumer.Consume(cancellationToken);

                    _logger.LogInformation("Receive {Key} : {Value}", cr.Message.Key, cr.Message.Value);
                    Console.WriteLine($"Received message at offset {cr.Message.Key}: {cr.Message.Value}");
                    var data = JsonSerializer.Deserialize<BKafka>(cr.Message.Value);

                  
                }
                catch (OperationCanceledException e)
                {
                    // Log the cancellation
                    _logger.LogError("Operation canceled : {Message}", e.Message);
                }
                catch (ConsumeException e)
                {
                    // Consumer errors should generally be ignored (or logged) unless fatal.
                    _logger.LogError($"Consume error: {e.Error.Reason}");

                    if (e.Error.IsFatal)
                    {
                        // https://github.com/edenhill/librdkafka/blob/master/INTRODUCTION.md#fatal-consumer-errors
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError("Unexpected error: {Message}", e.Message + (e.InnerException?.Message ?? ""));
                }
            }
        }


     
        public override void Dispose()
        {
            _EdwServiceConsumer.Close();
            _EdwServiceConsumer.Dispose();
           
            base.Dispose();
        }
    }
}

