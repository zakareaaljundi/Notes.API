using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tags.API.Data;
using Microsoft.EntityFrameworkCore;

namespace Tags.API.Events
{
    public class NoteCreatedConsumer : BackgroundService
    {
        private const string TopicName = "note-created";
        private const string GroupId = "tags-api";

        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;

        public NoteCreatedConsumer(
            IConfiguration configuration,
            IServiceScopeFactory scopeFactory)
        {
            _configuration = configuration;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _configuration.GetConnectionString("kafka"),
                GroupId = GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            using var consumer = new ConsumerBuilder<string, string>(config).Build();
            consumer.Subscribe(TopicName);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Poll with timeout instead of blocking forever
                    var result = consumer.Consume(TimeSpan.FromMilliseconds(500));

                    if (result == null)
                    {
                        await Task.Delay(500, stoppingToken); // avoid busy loop
                        continue;
                    }

                    var noteCreatedEvent =
                        JsonSerializer.Deserialize<NoteCreatedEvent>(result.Message.Value);

                    if (noteCreatedEvent != null)
                    {
                        await HandleEventAsync(noteCreatedEvent);
                        consumer.Commit(result);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Kafka consumer error: {ex}");
                    await Task.Delay(1000, stoppingToken); // backoff on error
                }
            }
        }


        private async Task HandleEventAsync(NoteCreatedEvent evt)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TagsDbContext>();

            // Example: store note reference
            var exists = await dbContext.Notes.AnyAsync(n => n.Id == evt.NoteId);
            if (exists) return;

            dbContext.Notes.Add(new Note
            {
                Id = evt.NoteId,
                CreatedAt = evt.CreatedAt
            });

            await dbContext.SaveChangesAsync();
        }
    }
}
