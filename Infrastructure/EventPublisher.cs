using System.Text.Json;
using Agenda.API.Domain;
using Confluent.Kafka;

namespace Agenda.API.Infrastructure
{
    public class ReminderPublisher
    {
        private readonly IProducer<Null, string> _producer;
        private readonly string _topic = "reminder-created";

        public ReminderPublisher()
        {
            var config = new ProducerConfig { BootstrapServers = "host.docker.internal:9092" };

            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task PublishReminderCreated(ReminderCreatedEvent evt)
        {
            var json = JsonSerializer.Serialize(evt);

            await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = json });
        }
    }
}
