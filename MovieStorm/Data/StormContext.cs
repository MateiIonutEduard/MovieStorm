using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MovieStorm.Data
{
    public class StormContext : DbContext
    {
        public StormContext(DbContextOptions<StormContext> options) : base(options)
        { }

        public DbSet<User> User { get; set; }

        public DbSet<Movie> Movie { get; set; }

        public DbSet<Review> Review { get; set; }

        public DbSet<Subtitle> Subtitle { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.username).IsRequired();
                entity.Property(u => u.password).IsRequired();
                entity.Property(u => u.address).IsRequired();
                entity.Property(u => u.username).IsRequired();
                entity.Property(u => u.logo).IsRequired();
                entity.Property(u => u.token);
                entity.Property(u => u.admin).IsRequired();
            });

            modelBuilder.Entity<Movie>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.name).IsRequired();
                entity.Property(m => m.genre).IsRequired();
                entity.Property(m => m.released).IsRequired();
                entity.Property(m => m.description).IsRequired();
                entity.Property(m => m.preview).IsRequired();
                entity.Property(m => m.views).IsRequired();
                entity.Property(m => m.user_id).IsRequired();

                entity.HasOne(m => m.User)
                    .WithMany(u => u.Movies);

                entity.Property(m => m.path).IsRequired();
            });

            modelBuilder.Entity<Subtitle>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.code).IsRequired();

                entity.Property(s => s.path).IsRequired();
                entity.Property(s => s.movie_id).IsRequired();

                entity.HasOne(s => s.Movie)
                .WithMany(m => m.Subtitles);
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.rating).IsRequired();
                entity.Property(r => r.content).IsRequired();
                entity.Property(r => r.rating).IsRequired();

                entity.Property(r => r.user_id).IsRequired();
                entity.Property(r => r.movie_id).IsRequired();

                entity.HasOne(r => r.Movie)
                    .WithMany(m => m.Reviews);

                entity.HasOne(r => r.User)
                    .WithMany(u => u.Reviews);
            });
        }
    }
}
