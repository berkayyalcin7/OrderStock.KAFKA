using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Configuration;
using Shared.Event;

namespace Stock.API.Services
{
    public class Bus(IConfiguration configuration):IBus
    {
        public ConsumerConfig GetConsumerConfig(string groupId)
        {
            return new()
            {
                BootstrapServers = configuration.GetSection("BusSettings").GetSection("Kafka")["BootstrapServers"],
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                // Kafka mesajları alır ve offsett değerlerini kaydırır . (Doğru veya yanlış mesaj değeri bakılmaksızın bu gerçekleşir - true durumunda)
                // False durumunda manuel olarak offsett yapıyoruz. Böylece hatayı yakalayabiliriz.
                EnableAutoCommit = false

            };
        }

    }
}
