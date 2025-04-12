using Confluent.Kafka;
using Kafka.Consumer;
using Shared.Event;
using Shared.Event.Events;
using Stock.API.Services;

namespace Stock.API.BackgroundServices
{
    public class OrderCreatedEventConsumerBackgroundService(IBus bus,ILogger<OrderCreatedEventConsumerBackgroundService> logger) : BackgroundService
    {
        private IConsumer<string, OrderCreatedEvent>? _consumer;
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _consumer = new ConsumerBuilder<string, OrderCreatedEvent>(bus.GetConsumerConfig(BusConsts.OrderCreatedEventGroupId))
             .SetValueDeserializer(new CustomValueDeserializer<OrderCreatedEvent>())
             .Build();

            _consumer.Subscribe(BusConsts.OrderCreatedEventTopicName);

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            // Uygulama kapanırken 
            while (!stoppingToken.IsCancellationRequested)
            {
                // OrderCreatedEvent için Deseralize işlemleri yapılacak.

                // timeout verdik , yoksa mesaj gelene kadar bekler
                var consumeMessage = _consumer!.Consume(200);

                if (consumeMessage != null)
                {
                    try
                    {
                        var orderCreatedEvent = consumeMessage.Message.Value;
                        //  gelen mesaj içindeki değerler init olduğu için direkt ordercreatedevent üstünden tutarı değiştiremeyiz.
                        // bu şekilde yeni değişkene değer atayabiliriz
                        //var a = orderCreatedEvent with { totalPrice = 500 };
                        // Stoktan düşme işlemi yapıldığını varsayırozu.
                        logger.LogInformation($"User Id  : {orderCreatedEvent.UserId} , Order Code : {orderCreatedEvent.OrderCode} " +
                            $", Total Price : {orderCreatedEvent.totalPrice}");

                        // Commit ediyoruz. Acknowladge bu şekilde.
                        _consumer.Commit(consumeMessage);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex.Message);
                        throw;
                    }

                }

                await Task.Delay(20,stoppingToken);
            }
        }
    }
}
