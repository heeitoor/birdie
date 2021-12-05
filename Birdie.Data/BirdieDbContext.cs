using Birdie.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace Birdie.Data
{
    internal class BirdieDbContext : DbContext
    {
        #region DbSets

        public DbSet<User> User { get; set; }
        public DbSet<Room> Room { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<Stock> Stock { get; set; }

        #endregion

        public BirdieDbContext(DbContextOptions dbContextOptions)
            : base(dbContextOptions)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(x => x.Id).Metadata.IsPrimaryKey();
            modelBuilder.Entity<User>().Property(x => x.Id).IsRequired();
            modelBuilder.Entity<User>().Property(x => x.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<User>().Property(x => x.UserName).HasMaxLength(15).IsRequired();
            modelBuilder.Entity<User>().Property(x => x.Password).HasMaxLength(300).IsRequired();
            modelBuilder.Entity<User>().HasMany(x => x.Room).WithOne(x => x.User);
            modelBuilder.Entity<User>().HasMany(x => x.Message).WithOne(x => x.User);
            modelBuilder.Entity<User>().HasMany(x => x.Stock).WithOne(x => x.User);
            modelBuilder.Entity<User>()
                .Property(x => x.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("current_timestamp");

            modelBuilder.Entity<Room>().HasKey(x => x.Id).Metadata.IsPrimaryKey();
            modelBuilder.Entity<Room>().Property(x => x.Id).IsRequired();
            modelBuilder.Entity<Room>().Property(x => x.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Room>()
                .Property(x => x.Identifier)
                .IsRequired()
                .HasDefaultValue(Guid.NewGuid());
            modelBuilder.Entity<Room>().Property(x => x.UserId).IsRequired();
            modelBuilder.Entity<Room>().Property(x => x.Name).HasMaxLength(15).IsRequired();
            modelBuilder.Entity<Room>()
                .Property(x => x.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("current_timestamp");
            modelBuilder.Entity<Room>().HasOne(x => x.User).WithMany(x => x.Room);

            modelBuilder.Entity<Message>().HasKey(x => x.Id).Metadata.IsPrimaryKey();
            modelBuilder.Entity<Message>().Property(x => x.Id).IsRequired();
            modelBuilder.Entity<Message>().Property(x => x.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Message>().Property(x => x.UserId).IsRequired();
            modelBuilder.Entity<Message>().Property(x => x.RoomId).IsRequired();
            modelBuilder.Entity<Message>().Property(x => x.Content).IsRequired().HasMaxLength(2000);
            modelBuilder.Entity<Message>()
                .Property(x => x.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("current_timestamp");
            modelBuilder.Entity<Message>().HasOne(x => x.User).WithMany(x => x.Message);
            modelBuilder.Entity<Message>().HasOne(x => x.Room).WithMany(x => x.Message);

            modelBuilder.Entity<Stock>().HasKey(x => x.Id).Metadata.IsPrimaryKey();
            modelBuilder.Entity<Stock>().Property(x => x.Id).IsRequired();
            modelBuilder.Entity<Stock>().Property(x => x.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Stock>().Property(x => x.Symbol).IsRequired().HasMaxLength(20);
            modelBuilder.Entity<Stock>().HasIndex(x => x.Symbol).IsUnique();
            modelBuilder.Entity<Stock>().Property(x => x.Date).IsRequired();
            modelBuilder.Entity<Stock>().Property(x => x.Open).IsRequired();
            modelBuilder.Entity<Stock>().Property(x => x.High).IsRequired();
            modelBuilder.Entity<Stock>().Property(x => x.Low).IsRequired();
            modelBuilder.Entity<Stock>().Property(x => x.Close).IsRequired();
            modelBuilder.Entity<Stock>()
                .Property(x => x.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("current_timestamp");

            // I'm adding this here just for simplicity sake, otherwise I wouldn't
            modelBuilder.Entity<User>().HasData(new[]
            {
                new User
                {
                    Id = 1,
                    UserName = "bot",
                    Password = "JNGuicG0l9amrBO1wj1MuA==",
                    UpdatedAt = DateTimeOffset.Now,
                },
                new User
                {
                    Id = 2,
                    UserName = "admin",
                    Password = "JNGuicG0l9amrBO1wj1MuA==",
                    UpdatedAt = DateTimeOffset.Now,
                }
            });

            modelBuilder.Entity<Room>().HasData(new[]
            {
                new Room
                {
                    Id = 1,
                    Name = "Waiting room",
                    Identifier = Guid.NewGuid(),
                    UserId = 2,
                    UpdatedAt = DateTimeOffset.Now
                },
                new Room
                {
                    Id = 2,
                    Name = "Cool room",
                    Identifier = Guid.NewGuid(),
                    UserId = 2,
                    UpdatedAt = DateTimeOffset.Now
                },
                new Room
                {
                    Id = 3,
                    Name = "Cactus room",
                    Identifier = Guid.NewGuid(),
                    UserId = 2,
                    UpdatedAt = DateTimeOffset.Now
                },
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
