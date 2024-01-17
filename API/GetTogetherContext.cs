using API.Helpers;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API;

public partial class GetTogetherContext : DbContext
{
    public GetTogetherContext() { }

    public GetTogetherContext(DbContextOptions<GetTogetherContext> options)
        : base(options) { }

    public virtual DbSet<Activity> Activities { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventParticipant> EventParticipants { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserActivity> UserActivities { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySql(EnvHelper.GetDatabaseConnectionString(), ServerVersion.Parse(EnvHelper.GetEnv(EnvVariable.DbVersion)));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb3_general_ci")
            .HasCharSet("utf8mb3");

        modelBuilder.Entity<Activity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("ID");
            entity.Property(e => e.IconClassName).HasColumnName("iconClassName");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.ActivityId, "Event_Activity");

            entity.HasIndex(e => e.OrganizerId, "Event_Organizer");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("ID");
            entity.Property(e => e.ActivityId)
                .HasColumnType("int(11)")
                .HasColumnName("activityID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("createdAt");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .HasColumnName("location");
            entity.Property(e => e.MaxParticipants)
                .HasColumnType("int(11)")
                .HasColumnName("maxParticipants");
            entity.Property(e => e.OrganizerId)
                .HasColumnType("int(11)")
                .HasColumnName("organizerID");
            entity.Property(e => e.Title)
                .HasMaxLength(150)
                .HasColumnName("title");
            entity.Property(e => e.Visibility)
                .HasDefaultValueSql("'public'")
                .HasColumnType("enum('private','public')")
                .HasColumnName("visibility");

            entity.HasOne(d => d.Activity).WithMany(p => p.Events)
                .HasForeignKey(d => d.ActivityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Event_Activity");

            entity.HasOne(d => d.Organizer).WithMany(p => p.Events)
                .HasForeignKey(d => d.OrganizerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Event_Organizer");
        });

        modelBuilder.Entity<EventParticipant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.EventId, "EventParticipants_Event");

            entity.HasIndex(e => e.ParticipantId, "EventParticipants_Participant");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("ID");
            entity.Property(e => e.EventId)
                .HasColumnType("int(11)")
                .HasColumnName("eventID");
            entity.Property(e => e.ParticipantId)
                .HasColumnType("int(11)")
                .HasColumnName("participantID");
            entity.Property(e => e.Status)
                .HasColumnType("enum('going','maybe','not going')")
                .HasColumnName("status");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.Event).WithMany(p => p.EventParticipants)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientCascade)
                .HasConstraintName("EventParticipants_Event");

            entity.HasOne(d => d.Participant).WithMany(p => p.EventParticipants)
                .HasForeignKey(d => d.ParticipantId)
                .OnDelete(DeleteBehavior.ClientCascade)
                .HasConstraintName("EventParticipants_Participant");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.EventId, "Notifications_Event");

            entity.HasIndex(e => e.ParticipantId, "Notifications_Participant");

            entity.HasIndex(e => e.OrganizerId, "Notifications_Organizer");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("ID");
            entity.Property(e => e.EventId)
                .HasColumnType("int(11)")
                .HasColumnName("eventID");
            entity.Property(e => e.ParticipantId)
                .HasColumnType("int(11)")
                .HasColumnName("participantID");
            entity.Property(e => e.Status)
                .HasColumnType("enum('joined','updated','left')")
                .HasColumnName("status");
            entity.Property(e => e.OrganizerId)
                .HasColumnType("int(11)")
                .HasColumnName("organizerID");

            entity.HasOne(d => d.Event).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.EventId)
                .HasConstraintName("Notifications_Event");

            entity.HasOne(d => d.Participant).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.ParticipantId)
                .HasConstraintName("Notifications_Participant");

            entity.HasOne(d => d.Organizer).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.OrganizerId)
                .OnDelete(DeleteBehavior.ClientCascade)
                .HasConstraintName("Notifications_Organizer");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("ID");
            entity.Property(e => e.AppleId)
                .HasMaxLength(255)
                .HasColumnName("appleID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("createdAt");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.FacebookId)
                .HasMaxLength(255)
                .HasColumnName("facebookID");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("firstName");
            entity.Property(e => e.GoogleId)
                .HasMaxLength(255)
                .HasColumnName("googleID");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("lastName");
            entity.Property(e => e.ProfileImageUrl)
                .HasColumnType("text")
                .HasColumnName("profileImageUrl");
            entity.Property(e => e.RefreshToken)
                .HasColumnType("text")
                .HasColumnName("refreshToken");
            entity.Property(e => e.TwitterId)
                .HasMaxLength(255)
                .HasColumnName("twitterID");
        });

        modelBuilder.Entity<UserActivity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.ActivityId, "userActivities_activity_fk");

            entity.HasIndex(e => e.UserId, "userActivities_user_fk");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("ID");
            entity.Property(e => e.ActivityId)
                .HasColumnType("int(11)")
                .HasColumnName("activityID");
            entity.Property(e => e.UserId)
                .HasColumnType("int(11)")
                .HasColumnName("userID");

            entity.HasOne(d => d.Activity).WithMany(p => p.UserActivities)
                .HasForeignKey(d => d.ActivityId)
                .OnDelete(DeleteBehavior.ClientCascade)
                .HasConstraintName("userActivities_activity_fk");

            entity.HasOne(d => d.User).WithMany(p => p.UserActivities)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientCascade)
                .HasConstraintName("userActivities_user_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
