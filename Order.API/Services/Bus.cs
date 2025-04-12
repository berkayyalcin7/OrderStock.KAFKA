using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Configuration;
using Shared.Event;

namespace Order.API.Services
{
    public class Bus(IConfiguration configuration,ILogger<Bus> logger) : IBus
    {

        private readonly ProducerConfig config = new()
        {
            // appsettings all
            BootstrapServers = configuration.GetSection("BusSettings").GetSection("Kafka")["BootstrapServers"],
            Acks = Acks.All,
            // 5 kez denemeden sonra hala bağlanamaz ise Hata dön.
            // Timeout belirlemek ise daha uygundur
            //MessageSendMaxRetries=5,
            // 10 saniye içinde dönmez ise hata dön.
            MessageTimeoutMs = 10000,
            // Bu property default'u true dur. Manual oluşturmadan Apache Kafka ' ya bırakabiliriz.
            AllowAutoCreateTopics = true,
        };

        public async Task<bool> Publish<T1, T2>(T1 key, T2 value, string topicOrQueueName)
        {
            using var producer = new ProducerBuilder<T1, T2>(config)
                .SetKeySerializer(new CustomKeySerializer<T1>())
                .SetValueSerializer(new CustomValueSerializer<T2>())
                .Build();

            var message = new Message<T1, T2>()
            {
                Key = key,
                Value = value
            };

            var result = await producer.ProduceAsync(topicOrQueueName, message);

            return result.Status == PersistenceStatus.Persisted;
        }

        public async Task CreateTopicOrQueue(List<string> topicOrQueueNameList)
        {
            using var adminClient = new AdminClientBuilder(
                new AdminClientConfig()
                {
                    // 3 broker olsaydı , 3 tane ifadeyi tek tek yazacaktık
                    BootstrapServers = configuration.GetSection("BusSettings").GetSection("Kafka")["BootstrapServers"],
                }).Build();

            try
            {
                foreach (var topicOrQueue in topicOrQueueNameList)
                {
                    var config = new Dictionary<string, string>()
                {
                    { "message.timestamp.type","LogAppendTime" },
                };

                    await adminClient.CreateTopicsAsync(new[]
                    {
			// 1 broker olduğu için 1 replika veriyoruz.
			new TopicSpecification(){
                Name=topicOrQueue,NumPartitions=5,
                ReplicationFactor=1,
                Configs= config
            }
        });
                    logger.LogInformation("Topic oluştu : " + topicOrQueue);
                 
                }

            }
            catch (Exception ex)
            {
                logger.LogWarning(ex.Message);
            }
        }
    }
}
