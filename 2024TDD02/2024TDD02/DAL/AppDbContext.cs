using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Databases
{
    public class AppDbContext : DbContext
    {
        public DbSet<CopyTransaction> CopyTransactions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }               // New DbSet for Events
        public DbSet<EventTicket> EventTickets { get; set; }   // New DbSet for EventTickets

        // Seed data for Users
        public static User[] UserList = new[]
        {
            new User
            {
                Id = 1,
                Name = "Alexandre Salamin",
                Email = "alexandre.salamin@mail.com",
                Balance = 50
            },
            new User
            {
                Id = 2,
                Name = "Jonathan Araujo",
                Email = "jonathan.araujo@mail.com",
                Balance = 100
            },
            new User
            {
                Id = 3,
                Name = "Adrien Destefani",
                Email = "adrien.destefani@mail.com",
                Balance = 80
            }
        };

        // Seed data for Copy Transactions
        public static CopyTransaction[] TransactionList = new[]
        {
            new CopyTransaction(1, 1, 10, 1.00m, DateTime.UtcNow),
            new CopyTransaction(2, 1, 5, 0.50m, DateTime.UtcNow)
        };

        // Seed data for Events
        public static Event[] EventList = new[]
        {
            new Event
            {
                Id = "E1",
                Name = "University Concert",
                Date = DateTime.UtcNow.AddDays(15),
                AvailableTickets = 100,
                TicketPrice = 50.0m
            },
            new Event
            {
                Id = "E2",
                Name = "Science Seminar",
                Date = DateTime.UtcNow.AddDays(10),
                AvailableTickets = 50,
                TicketPrice = 25.0m
            }
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

            // Configure CopyTransaction entity
            modelBuilder.Entity<CopyTransaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
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
                entity.Property(et => et.PurchaseDate).IsRequired(); // Simply mark it as required
            });


            // Seed initial data
            modelBuilder.Entity<User>().HasData(UserList);
            modelBuilder.Entity<CopyTransaction>().HasData(TransactionList);
            modelBuilder.Entity<Event>().HasData(EventList); // Seed data for Events
        }
    }
}
