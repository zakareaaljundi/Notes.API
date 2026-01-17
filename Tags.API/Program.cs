using Microsoft.EntityFrameworkCore;
using Tags.API.Data;
using Tags.API.Events;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDbContext<TagsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("tagsdb")));
builder.EnrichNpgsqlDbContext<TagsDbContext>();

builder.Services.AddHostedService<NoteCreatedConsumer>();

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Apply EF Migrations
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<TagsDbContext>();
    dbContext.Database.Migrate();

}

app.UseHttpsRedirection();

app.Run();