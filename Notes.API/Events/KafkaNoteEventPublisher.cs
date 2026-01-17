using Confluent.Kafka;
using System.Text.Json;

namespace Notes.API.Events
{
    public class KafkaNoteEventPublisher : INoteEventPublisher
    {
        private const string TopicName = "note-created";

        private readonly IProducer<string, string> _producer;

        public KafkaNoteEventPublisher(IConfiguration configuration)
        {
            var bootstrapServers = configuration.GetConnectionString("kafka");

            var config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers
            };

            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task NoteCreatedAsync(NoteCreatedEvent noteCreatedEvent)
        {
            var message = new Message<string, string>
            {
                Key = noteCreatedEvent.NoteId.ToString(),
                Value = JsonSerializer.Serialize(noteCreatedEvent)
            };

            await _producer.ProduceAsync(TopicName, message);
        }
    }
}
