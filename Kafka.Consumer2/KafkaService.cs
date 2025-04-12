using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kafka.Consumer2
{
    internal class KafkaService
    {
        // Rabbit MQ ' da Push mantığı vardır.

        // Kafkada Pull mantığı vardır . Mesajları Consumer olarak biz alırız.
        internal async Task ConsumeSimpleMessageWithNullKey(string topicName)
        {

            // Offsett en sonda olduğu için daha önce Publisher'dan okuduklarını tekrar okumaz.
            var config = new ConsumerConfig()
            {
                BootstrapServers = "localhost:9094",
                GroupId = "use-case-1-group-2",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            // Latest dediğimizde bağlandığımız andan itibaren mesajları okumaya başlarız.
            //var config1 = new ConsumerConfig()
            //{
            //    BootstrapServers = "localhost:9094",
            //    GroupId = "use-case-1-group-2",
            //    AutoOffsetReset = AutoOffsetReset.Latest
            //};

            var consumer = new ConsumerBuilder<Null, string>(config).Build();

            consumer.Subscribe(topicName);

            while (true)
            {
                // timeout verdik , yoksa mesaj gelene kadar bekler
                var consumeMessage = consumer.Consume(5000);

                if (consumeMessage != null)
                {
                    Console.WriteLine("Gelen mesaj : " + consumeMessage.Message.Value);
                }

                await Task.Delay(200);
            }

        }
    }
}
