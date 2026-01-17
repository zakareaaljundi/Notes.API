namespace Notes.API.Events
{
    public class NoOpNoteEventPublisher : INoteEventPublisher
    {
        private readonly ILogger<NoOpNoteEventPublisher> _logger;

        public NoOpNoteEventPublisher(ILogger<NoOpNoteEventPublisher> logger)
        {
            _logger = logger;
        }

        public Task NoteCreatedAsync(NoteCreatedEvent noteCreatedEvent)
        {
            _logger.LogInformation(
                "NoteCreated event fired for NoteId={NoteId}",
                noteCreatedEvent.NoteId
            );

            return Task.CompletedTask;
        }
    }
}
