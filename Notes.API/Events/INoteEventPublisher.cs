namespace Notes.API.Events
{
    public interface INoteEventPublisher
    {
        Task NoteCreatedAsync(NoteCreatedEvent noteCreatedEvent);
    }
}
