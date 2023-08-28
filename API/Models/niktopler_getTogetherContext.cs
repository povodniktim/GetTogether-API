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

    public virtual DbSet<EventComments> EventComments { get; set; }

    public virtual DbSet<EventParticipants> EventParticipants { get; set; }

    public virtual DbSet<EventPhotos> EventPhotos { get; set; }

    public virtual DbSet<Events> Events { get; set; }

    public virtual DbSet<Users> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySql("name=ConnectionStrings:niktopler_GetTogether", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.6.15-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("latin1_swedish_ci")
            .HasCharSet("latin1");

        modelBuilder.Entity<EventComments>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.EventID, "EventComments_Event");

            entity.HasIndex(e => e.UserID, "EventComments_User");

            entity.Property(e => e.ID)
                .ValueGeneratedNever()
                .HasColumnType("int(11)");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.EventID).HasColumnType("int(11)");
            entity.Property(e => e.Text).HasColumnType("text");
            entity.Property(e => e.UserID).HasColumnType("int(11)");

            entity.HasOne(d => d.Event).WithMany(p => p.EventComments)
                .HasForeignKey(d => d.EventID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("EventComments_Event");

            entity.HasOne(d => d.User).WithMany(p => p.EventComments)
                .HasForeignKey(d => d.UserID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("EventComments_User");
        });

        modelBuilder.Entity<EventParticipants>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.EventID, "EventParticipants_Event");

            entity.HasIndex(e => e.ParticipantID, "EventParticipants_Participant");

            entity.Property(e => e.ID)
                .ValueGeneratedNever()
                .HasColumnType("int(11)");
            entity.Property(e => e.EventID).HasColumnType("int(11)");
            entity.Property(e => e.ParticipantID).HasColumnType("int(11)");

            entity.HasOne(d => d.Event).WithMany(p => p.EventParticipants)
                .HasForeignKey(d => d.EventID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("EventParticipants_Event");

            entity.HasOne(d => d.Participant).WithMany(p => p.EventParticipants)
                .HasForeignKey(d => d.ParticipantID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("EventParticipants_Participant");
        });

        modelBuilder.Entity<EventPhotos>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.EventID, "EventPhotos_Event");

            entity.Property(e => e.ID)
                .ValueGeneratedNever()
                .HasColumnType("int(11)");
            entity.Property(e => e.EventID).HasColumnType("int(11)");
            entity.Property(e => e.URL).HasMaxLength(255);
            entity.Property(e => e.UploadedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Event).WithMany(p => p.EventPhotos)
                .HasForeignKey(d => d.EventID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("EventPhotos_Event");
        });

        modelBuilder.Entity<Events>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.HasIndex(e => e.OrganizerID, "Event_Organizer");

            entity.Property(e => e.ID)
                .ValueGeneratedNever()
                .HasColumnType("int(11)");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Location).HasColumnType("text");
            entity.Property(e => e.MaxParticipants)
                .HasDefaultValueSql("'5'")
                .HasColumnType("int(11)");
            entity.Property(e => e.OrganizerID).HasColumnType("int(11)");
            entity.Property(e => e.Title).HasMaxLength(125);

            entity.HasOne(d => d.Organizer).WithMany(p => p.Events)
                .HasForeignKey(d => d.OrganizerID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Event_Organizer");
        });

        modelBuilder.Entity<Users>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.Property(e => e.ID)
                .ValueGeneratedNever()
                .HasColumnType("int(11)");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastLoggedInAt).HasColumnType("datetime");
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
