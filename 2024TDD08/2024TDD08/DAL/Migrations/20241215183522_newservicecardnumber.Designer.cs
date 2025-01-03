﻿// <auto-generated />
using System;
using DAL.Databases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DAL.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241215183522_newservicecardnumber")]
    partial class newservicecardnumber
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DAL.Models.Event", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AvailableTickets")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("TicketPrice")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.ToTable("Events");

                    b.HasData(
                        new
                        {
                            Id = "E1",
                            AvailableTickets = 100,
                            Date = new DateTime(2024, 12, 30, 18, 35, 22, 41, DateTimeKind.Utc).AddTicks(8308),
                            Name = "University Concert",
                            TicketPrice = 50.0m
                        },
                        new
                        {
                            Id = "E2",
                            AvailableTickets = 50,
                            Date = new DateTime(2024, 12, 25, 18, 35, 22, 41, DateTimeKind.Utc).AddTicks(8860),
                            Name = "Science Seminar",
                            TicketPrice = 25.0m
                        },
                        new
                        {
                            Id = "E3",
                            AvailableTickets = 75,
                            Date = new DateTime(2024, 12, 20, 18, 35, 22, 41, DateTimeKind.Utc).AddTicks(8865),
                            Name = "Art Exhibition",
                            TicketPrice = 20.0m
                        },
                        new
                        {
                            Id = "E4",
                            AvailableTickets = 60,
                            Date = new DateTime(2025, 1, 4, 18, 35, 22, 41, DateTimeKind.Utc).AddTicks(8867),
                            Name = "Tech Workshop",
                            TicketPrice = 30.0m
                        },
                        new
                        {
                            Id = "E5",
                            AvailableTickets = 90,
                            Date = new DateTime(2024, 12, 22, 18, 35, 22, 41, DateTimeKind.Utc).AddTicks(8868),
                            Name = "Film Screening",
                            TicketPrice = 15.0m
                        },
                        new
                        {
                            Id = "E6",
                            AvailableTickets = 120,
                            Date = new DateTime(2025, 1, 9, 18, 35, 22, 41, DateTimeKind.Utc).AddTicks(8870),
                            Name = "Sports Gala",
                            TicketPrice = 40.0m
                        },
                        new
                        {
                            Id = "E7",
                            AvailableTickets = 150,
                            Date = new DateTime(2025, 1, 14, 18, 35, 22, 41, DateTimeKind.Utc).AddTicks(8871),
                            Name = "Music Festival",
                            TicketPrice = 70.0m
                        },
                        new
                        {
                            Id = "E8",
                            AvailableTickets = 40,
                            Date = new DateTime(2024, 12, 27, 18, 35, 22, 41, DateTimeKind.Utc).AddTicks(8873),
                            Name = "Literature Meetup",
                            TicketPrice = 10.0m
                        },
                        new
                        {
                            Id = "E9",
                            AvailableTickets = 80,
                            Date = new DateTime(2025, 1, 2, 18, 35, 22, 41, DateTimeKind.Utc).AddTicks(8874),
                            Name = "Business Conference",
                            TicketPrice = 60.0m
                        },
                        new
                        {
                            Id = "E10",
                            AvailableTickets = 55,
                            Date = new DateTime(2025, 1, 6, 18, 35, 22, 41, DateTimeKind.Utc).AddTicks(8875),
                            Name = "Charity Auction",
                            TicketPrice = 35.0m
                        });
                });

            modelBuilder.Entity("DAL.Models.EventTicket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("EventId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("PurchaseDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("ServiceCard")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("EventTickets");
                });

            modelBuilder.Entity("DAL.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Balance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ServiceCard")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Balance = 1500m,
                            Name = "Alessandro Chiolini",
                            ServiceCard = "1"
                        },
                        new
                        {
                            Id = 2,
                            Balance = 1000m,
                            Name = "Julien Blanch-Lanao",
                            ServiceCard = "2"
                        },
                        new
                        {
                            Id = 3,
                            Balance = 1200m,
                            Name = "Gian-Luca Gloor",
                            ServiceCard = "3"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
