﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Server.Helpers;

#nullable disable

namespace Server.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20220822162320_AddPhoto")]
    partial class AddPhoto
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Server.Entities.Dmd", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<List<int>>("MeditationsId")
                        .IsRequired()
                        .HasColumnType("integer[]");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("DMDs");
                });

            modelBuilder.Entity("Server.Entities.Meditation", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("id"));

                    b.Property<int>("CountDay")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<bool>("IsSubscribed")
                        .HasColumnType("boolean");

                    b.Property<string>("Language")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("Time")
                        .HasColumnType("integer");

                    b.Property<int>("TypeMeditation")
                        .HasColumnType("integer");

                    b.HasKey("id");

                    b.ToTable("Meditations");
                });

            modelBuilder.Entity("Server.Entities.Notification", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("ExpoToken")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsSubscribedToNotification")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastNotification")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("NotificationFrequency")
                        .HasColumnType("integer");

                    b.HasKey("UserId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("Server.Entities.Payment.Payment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("Confirm")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("PaymentDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("Server.Entities.Subscribe", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<int>("TimeSubscribe")
                        .HasColumnType("integer");

                    b.Property<DateTime>("WhenSubscribe")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("UserId");

                    b.ToTable("Subscribes");
                });

            modelBuilder.Entity("Server.Entities.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Birthday")
                        .HasColumnType("text");

                    b.Property<int>("Category")
                        .HasColumnType("integer");

                    b.Property<DateTime>("DateTimeRegistration")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DisplayName")
                        .HasColumnType("text");

                    b.Property<int>("Gender")
                        .HasColumnType("integer");

                    b.Property<string>("NickName")
                        .HasColumnType("text");

                    b.Property<string>("Photo")
                        .HasColumnType("text");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<string>("Status")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Server.Entities.UserMeditation", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<int>("MeditationId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Time")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("UserId", "MeditationId");

                    b.HasIndex("MeditationId");

                    b.ToTable("UserMeditations");
                });

            modelBuilder.Entity("Server.Entities.UserMeditation", b =>
                {
                    b.HasOne("Server.Entities.Meditation", "Meditation")
                        .WithMany("UserMeditations")
                        .HasForeignKey("MeditationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Server.Entities.User", "User")
                        .WithMany("UserMeditations")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Meditation");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Server.Entities.Meditation", b =>
                {
                    b.Navigation("UserMeditations");
                });

            modelBuilder.Entity("Server.Entities.User", b =>
                {
                    b.Navigation("UserMeditations");
                });
#pragma warning restore 612, 618
        }
    }
}
