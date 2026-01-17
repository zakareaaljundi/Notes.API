using Microsoft.EntityFrameworkCore;
using Notes.API.Data;
using Notes.API.Events;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDbContext<NotesDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("notesdb")));
builder.EnrichNpgsqlDbContext<NotesDbContext>();

builder.Services.AddScoped<INoteEventPublisher, NoOpNoteEventPublisher>();
builder.Services.AddSingleton<INoteEventPublisher, KafkaNoteEventPublisher>();

builder.Services.AddOpenApi();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Apply EF Migrations
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<NotesDbContext>();
    dbContext.Database.Migrate();
}

app.UseHttpsRedirection();

app.Run();