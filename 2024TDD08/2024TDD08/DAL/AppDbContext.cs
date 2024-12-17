using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Databases
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }               // DbSet for Events
        public DbSet<EventTicket> EventTickets { get; set; }   // DbSet for EventTickets

        // Seed data for Users
        public static User[] UserList = new[]
        {
            new User
            {
                Id = 1,
                Name = "Alessandro Chiolini",
                Balance = 1500,
                ServiceCard = "CARD123456"
            },
            new User
            {
                Id = 2,
                Name = "Julien Blanch-Lanao",
                Balance = 1000,
                ServiceCard = "CARD654321"
            },
            new User
            {
                Id = 3,
                Name = "Gian-Luca Gloor",
                Balance = 1200,
                ServiceCard = "CARD987654"
            }
        };

        // Seed data for Events
        public static Event[] EventList = new[]
        {
            new Event { Id = "E1", Name = "University Concert", Date = DateTime.UtcNow.AddDays(15), AvailableTickets = 100, TicketPrice = 50.0m },
            new Event { Id = "E2", Name = "Science Seminar", Date = DateTime.UtcNow.AddDays(10), AvailableTickets = 50, TicketPrice = 25.0m },
            new Event { Id = "E3", Name = "Art Exhibition", Date = DateTime.UtcNow.AddDays(5), AvailableTickets = 75, TicketPrice = 20.0m },
            new Event { Id = "E4", Name = "Tech Workshop", Date = DateTime.UtcNow.AddDays(20), AvailableTickets = 60, TicketPrice = 30.0m },
            new Event { Id = "E5", Name = "Film Screening", Date = DateTime.UtcNow.AddDays(7), AvailableTickets = 90, TicketPrice = 15.0m },
            new Event { Id = "E6", Name = "Sports Gala", Date = DateTime.UtcNow.AddDays(25), AvailableTickets = 120, TicketPrice = 40.0m },
            new Event { Id = "E7", Name = "Music Festival", Date = DateTime.UtcNow.AddDays(30), AvailableTickets = 150, TicketPrice = 70.0m },
            new Event { Id = "E8", Name = "Literature Meetup", Date = DateTime.UtcNow.AddDays(12), AvailableTickets = 40, TicketPrice = 10.0m },
            new Event { Id = "E9", Name = "Business Conference", Date = DateTime.UtcNow.AddDays(18), AvailableTickets = 80, TicketPrice = 60.0m },
            new Event { Id = "E10", Name = "Charity Auction", Date = DateTime.UtcNow.AddDays(22), AvailableTickets = 55, TicketPrice = 35.0m }
        };

        // AppDbContext Constructor
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            // Optional: Add database connection string if not set in Program.cs
        }

        // Configure models and seed initial data
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Name).IsRequired();
                entity.Property(u => u.ServiceCard).IsRequired();
            });

            // Configure Event entity
            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.TicketPrice).HasPrecision(18, 2);
            });

            // Configure EventTicket entity
            modelBuilder.Entity<EventTicket>(entity =>
            {
                entity.HasKey(et => et.Id);
                entity.Property(et => et.PurchaseDate).IsRequired(); // Required field
            });

            // Seed initial data
            modelBuilder.Entity<User>().HasData(UserList);
            modelBuilder.Entity<Event>().HasData(EventList);
        }
    }
}
