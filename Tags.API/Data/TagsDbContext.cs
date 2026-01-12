using Microsoft.EntityFrameworkCore;

namespace Tags.API.Data
{
    public class TagsDbContext(DbContextOptions<TagsDbContext> options) : DbContext(options)
    {
        public DbSet<Tag> Tags { get; set; }

        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Color).IsRequired().HasMaxLength(7);
                entity.Property(e => e.NoteId).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.HasIndex(e => e.NoteId);
            });
        }
    }
}
