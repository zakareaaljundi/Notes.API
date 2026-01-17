namespace Tags.API.Events
{
    public record NoteCreatedEvent(
        Guid NoteId,
        DateTime CreatedAt
    );
}
