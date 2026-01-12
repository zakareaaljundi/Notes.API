using Microsoft.EntityFrameworkCore;
using Notes.API.Data;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDbContext<NotesDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("notesdb")));
builder.EnrichNpgsqlDbContext<NotesDbContext>();

builder.Services.AddOpenApi();

var app = builder.Build();

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