namespace API.Models;

public partial class Event
{
    public int Id { get; set; }

    public int OrganizerId { get; set; }

    public int ActivityId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime Date { get; set; }

    public string? Location { get; set; }

    public int? MaxParticipants { get; set; }

    public string? Visibility { get; set; }

    public virtual Activity Activity { get; set; } = null!;

    public virtual ICollection<EventParticipant> EventParticipants { get; set; } = new List<EventParticipant>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual User Organizer { get; set; } = null!;
}
