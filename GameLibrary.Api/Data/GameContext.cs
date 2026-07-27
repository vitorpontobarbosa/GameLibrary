using GameLibrary.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameLibrary.Api.Data
{
    public class GameContext : DbContext
    {
        public GameContext(DbContextOptions<GameContext> options) : base(options) { }

        public DbSet<Game> Games { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<GameOwnership> GameOwnerships { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GameOwnership>(entity =>
            {
                entity.HasIndex(o => new { o.GameId, o.UserId }).IsUnique();

                entity.HasOne(o => o.Game)
                    .WithMany()
                    .HasForeignKey(o => o.GameId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(o => o.User)
                    .WithMany()
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
