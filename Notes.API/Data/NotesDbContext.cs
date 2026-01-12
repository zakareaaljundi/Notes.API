using Microsoft.EntityFrameworkCore;

namespace Notes.API.Data
{
    public class NotesDbContext(DbContextOptions<NotesDbContext> options) : DbContext(options)
    {
        public DbSet<Note> Notes { get; set; }

        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Note>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired(false);
            });
        }
    }
}
