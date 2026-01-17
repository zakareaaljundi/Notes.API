using Microsoft.AspNetCore.Mvc;
using Notes.API.Data;
using Notes.API.Events;
using Notes.API.Models;

namespace Notes.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotesController : ControllerBase
    {
        private readonly NotesDbContext _dbContext;

        public NotesController(NotesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateNoteRequest request, [FromServices] INoteEventPublisher eventPublisher)
        {
            var note = new Note
            {
                Title = request.Title,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Notes.Add(note);
            await _dbContext.SaveChangesAsync();

            await eventPublisher.NoteCreatedAsync(
                new NoteCreatedEvent(note.Id, note.CreatedAt)
            );

            return Ok(note);
        }


    }
}
