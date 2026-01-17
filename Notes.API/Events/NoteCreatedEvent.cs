namespace Notes.API.Events
{
    public record NoteCreatedEvent(
        Guid NoteId,
        DateTime CreatedAt
    );
}
