﻿// <auto-generated />
using System;
using API;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace API.Migrations
{
    [DbContext(typeof(GetTogetherContext))]
    partial class GetTogetherContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseCollation("utf8mb3_general_ci")
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.HasCharSet(modelBuilder, "utf8mb3");

            modelBuilder.Entity("API.Models.Activity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int(11)")
                        .HasColumnName("ID");

                    b.Property<string>("IconClassName")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("iconClassName");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("PRIMARY");

                    b.ToTable("Activities");
                });

            modelBuilder.Entity("API.Models.Event", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int(11)")
                        .HasColumnName("ID");

                    b.Property<int>("ActivityId")
                        .HasColumnType("int(11)")
                        .HasColumnName("activityID");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasColumnName("createdAt")
                        .HasDefaultValueSql("current_timestamp()");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime")
                        .HasColumnName("date");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("location");

                    b.Property<int>("MaxParticipants")
                        .HasColumnType("int(11)")
                        .HasColumnName("maxParticipants");

                    b.Property<int>("OrganizerId")
                        .HasColumnType("int(11)")
                        .HasColumnName("organizerID");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("varchar(150)")
                        .HasColumnName("title");

                    b.Property<string>("Visibility")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("enum('private','public')")
                        .HasColumnName("visibility")
                        .HasDefaultValueSql("'public'");

                    b.HasKey("Id")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "ActivityId" }, "Event_Activity");

                    b.HasIndex(new[] { "OrganizerId" }, "Event_Organizer");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("API.Models.EventParticipant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int(11)")
                        .HasColumnName("ID");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime")
                        .HasColumnName("statusChangedAt");

                    b.Property<int>("EventId")
                        .HasColumnType("int(11)")
                        .HasColumnName("eventID");

                    b.Property<int>("ParticipantId")
                        .HasColumnType("int(11)")
                        .HasColumnName("participantID");

                    b.Property<string>("Status")
                        .HasColumnType("enum('going','maybe','not going')")
                        .HasColumnName("status");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "EventId" }, "EventParticipants_Event");

                    b.HasIndex(new[] { "ParticipantId" }, "EventParticipants_Participant");

                    b.ToTable("EventParticipants");
                });

            modelBuilder.Entity("API.Models.Notification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int(11)")
                        .HasColumnName("ID");

                    b.Property<int?>("EventId")
                        .HasColumnType("int(11)")
                        .HasColumnName("eventID");

                    b.Property<int>("OrganizerId")
                        .HasColumnType("int(11)")
                        .HasColumnName("organizerID");

                    b.Property<int?>("ParticipantId")
                        .HasColumnType("int(11)")
                        .HasColumnName("participantID");

                    b.Property<string>("Status")
                        .HasColumnType("enum('joined','updated','deleted')")
                        .HasColumnName("status");

                    b.HasKey("Id")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "EventId" }, "Notifications_Event");

                    b.HasIndex(new[] { "OrganizerId" }, "Notifications_Organizer");

                    b.HasIndex(new[] { "ParticipantId" }, "Notifications_Participant");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("API.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int(11)")
                        .HasColumnName("ID");

                    b.Property<string>("AppleId")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("appleID");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasColumnName("createdAt")
                        .HasDefaultValueSql("current_timestamp()");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("email");

                    b.Property<string>("FacebookId")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("facebookID");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("firstName");

                    b.Property<string>("GoogleId")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("googleID");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("lastName");

                    b.Property<string>("ProfileImageUrl")
                        .HasColumnType("text")
                        .HasColumnName("profileImageUrl");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("text")
                        .HasColumnName("refreshToken");

                    b.Property<string>("TwitterId")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("twitterID");

                    b.HasKey("Id")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "Email" }, "email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("API.Models.UserActivity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int(11)")
                        .HasColumnName("ID");

                    b.Property<int>("ActivityId")
                        .HasColumnType("int(11)")
                        .HasColumnName("activityID");

                    b.Property<int>("UserId")
                        .HasColumnType("int(11)")
                        .HasColumnName("userID");

                    b.HasKey("Id")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "ActivityId" }, "userActivities_activity_fk");

                    b.HasIndex(new[] { "UserId" }, "userActivities_user_fk");

                    b.ToTable("UserActivities");
                });

            modelBuilder.Entity("API.Models.Event", b =>
                {
                    b.HasOne("API.Models.Activity", "Activity")
                        .WithMany("Events")
                        .HasForeignKey("ActivityId")
                        .IsRequired()
                        .HasConstraintName("Event_Activity");

                    b.HasOne("API.Models.User", "Organizer")
                        .WithMany("Events")
                        .HasForeignKey("OrganizerId")
                        .IsRequired()
                        .HasConstraintName("Event_Organizer");

                    b.Navigation("Activity");

                    b.Navigation("Organizer");
                });

            modelBuilder.Entity("API.Models.EventParticipant", b =>
                {
                    b.HasOne("API.Models.Event", "Event")
                        .WithMany("EventParticipants")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired()
                        .HasConstraintName("EventParticipants_Event");

                    b.HasOne("API.Models.User", "Participant")
                        .WithMany("EventParticipants")
                        .HasForeignKey("ParticipantId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired()
                        .HasConstraintName("EventParticipants_Participant");

                    b.Navigation("Event");

                    b.Navigation("Participant");
                });

            modelBuilder.Entity("API.Models.Notification", b =>
                {
                    b.HasOne("API.Models.Event", "Event")
                        .WithMany("Notifications")
                        .HasForeignKey("EventId")
                        .HasConstraintName("Notifications_Event");

                    b.HasOne("API.Models.User", "Organizer")
                        .WithMany("Notifications")
                        .HasForeignKey("OrganizerId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired()
                        .HasConstraintName("Notifications_Organizer");

                    b.HasOne("API.Models.EventParticipant", "Participant")
                        .WithMany("Notifications")
                        .HasForeignKey("ParticipantId")
                        .HasConstraintName("Notifications_Participant");

                    b.Navigation("Event");

                    b.Navigation("Organizer");

                    b.Navigation("Participant");
                });

            modelBuilder.Entity("API.Models.UserActivity", b =>
                {
                    b.HasOne("API.Models.Activity", "Activity")
                        .WithMany("UserActivities")
                        .HasForeignKey("ActivityId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired()
                        .HasConstraintName("userActivities_activity_fk");

                    b.HasOne("API.Models.User", "User")
                        .WithMany("UserActivities")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired()
                        .HasConstraintName("userActivities_user_fk");

                    b.Navigation("Activity");

                    b.Navigation("User");
                });

            modelBuilder.Entity("API.Models.Activity", b =>
                {
                    b.Navigation("Events");

                    b.Navigation("UserActivities");
                });

            modelBuilder.Entity("API.Models.Event", b =>
                {
                    b.Navigation("EventParticipants");

                    b.Navigation("Notifications");
                });

            modelBuilder.Entity("API.Models.EventParticipant", b =>
                {
                    b.Navigation("Notifications");
                });

            modelBuilder.Entity("API.Models.User", b =>
                {
                    b.Navigation("EventParticipants");

                    b.Navigation("Events");

                    b.Navigation("Notifications");

                    b.Navigation("UserActivities");
                });
#pragma warning restore 612, 618
        }
    }
}
