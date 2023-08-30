using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

public partial class niktopler_getTogetherContext : DbContext
{
    public niktopler_getTogetherContext()
    {
    }

    public niktopler_getTogetherContext(DbContextOptions<niktopler_getTogetherContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Activities> Activities { get; set; }

    public virtual DbSet<EventParticipants> EventParticipants { get; set; }

    public virtual DbSet<Events> Events { get; set; }

    public virtual DbSet<Notifications> Notifications { get; set; }

    public virtual DbSet<Users> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySql("name=ConnectionStrings:niktopler_GetTogether", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.6.15-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("latin1_swedish_ci")
            .HasCharSet("latin1");

        modelBuilder.Entity<Activities>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID)
                .ValueGeneratedNever()
                .HasColumnType("int(11)");
            entity.Property(e => e.name).HasMaxLength(100);
        });

        modelBuilder.Entity<EventParticipants>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.eventID, "EventParticipants_Event");

            entity.HasIndex(e => e.participantID, "EventParticipants_Participant");

            entity.Property(e => e.ID)
                .ValueGeneratedNever()
                .HasColumnType("int(11)");
            entity.Property(e => e.eventID).HasColumnType("int(11)");
            entity.Property(e => e.participantID).HasColumnType("int(11)");
            entity.Property(e => e.status).HasColumnType("enum('going','maybe','not going')");
            entity.Property(e => e.statusChangedAt).HasColumnType("datetime");

            entity.HasOne(d => d._event).WithMany(p => p.EventParticipants)
                .HasForeignKey(d => d.eventID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("EventParticipants_Event");

            entity.HasOne(d => d.participant).WithMany(p => p.EventParticipants)
                .HasForeignKey(d => d.participantID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("EventParticipants_Participant");
        });

        modelBuilder.Entity<Events>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.activityID, "Event_Activity");

            entity.HasIndex(e => e.organizerID, "Event_Organizer");

            entity.Property(e => e.ID)
                .ValueGeneratedNever()
                .HasColumnType("int(11)");
            entity.Property(e => e.activityID).HasColumnType("int(11)");
            entity.Property(e => e.createdAt).HasColumnType("datetime");
            entity.Property(e => e.date).HasColumnType("datetime");
            entity.Property(e => e.description).HasColumnType("text");
            entity.Property(e => e.location).HasMaxLength(255);
            entity.Property(e => e.maxParticipants).HasColumnType("int(11)");
            entity.Property(e => e.organizerID).HasColumnType("int(11)");
            entity.Property(e => e.title).HasMaxLength(150);
            entity.Property(e => e.visibility)
                .HasDefaultValueSql("'public'")
                .HasColumnType("enum('private','public')");

            entity.HasOne(d => d.activity).WithMany(p => p.Events)
                .HasForeignKey(d => d.activityID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Event_Activity");

            entity.HasOne(d => d.organizer).WithMany(p => p.Events)
                .HasForeignKey(d => d.organizerID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Event_Organizer");
        });

        modelBuilder.Entity<Notifications>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.eventID, "Notifications_Event");

            entity.HasIndex(e => e.participantID, "Notifications_Participant");

            entity.HasIndex(e => e.userID, "Notifications_User");

            entity.Property(e => e.ID)
                .ValueGeneratedNever()
                .HasColumnType("int(11)");
            entity.Property(e => e.eventID).HasColumnType("int(11)");
            entity.Property(e => e.participantID).HasColumnType("int(11)");
            entity.Property(e => e.status)
                .HasDefaultValueSql("'not seen'")
                .HasColumnType("enum('seen','not seen','deleted')");
            entity.Property(e => e.userID).HasColumnType("int(11)");

            entity.HasOne(d => d._event).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.eventID)
                .HasConstraintName("Notifications_Event");

            entity.HasOne(d => d.participant).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.participantID)
                .HasConstraintName("Notifications_Participant");

            entity.HasOne(d => d.user).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.userID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Notifications_User");
        });

        modelBuilder.Entity<Users>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.email, "email").IsUnique();

            entity.Property(e => e.ID)
                .ValueGeneratedNever()
                .HasColumnType("int(11)");
            entity.Property(e => e.appleID).HasMaxLength(255);
            entity.Property(e => e.createdAt).HasColumnType("datetime");
            entity.Property(e => e.facebookID).HasMaxLength(255);
            entity.Property(e => e.firstName).HasMaxLength(50);
            entity.Property(e => e.googleID).HasMaxLength(255);
            entity.Property(e => e.lastName).HasMaxLength(50);
            entity.Property(e => e.profileImageUrl).HasColumnType("text");
            entity.Property(e => e.twitterID).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
