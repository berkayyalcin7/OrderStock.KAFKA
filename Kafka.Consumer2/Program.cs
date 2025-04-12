using Kafka.Consumer2;

Console.WriteLine("KAFKA CONSUMER - 2");

var kafkaService = new KafkaService();

await kafkaService.ConsumeSimpleMessageWithNullKey("use-case-1.2-topic");

Console.ReadLine();