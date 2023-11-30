using System.Text.Json.Serialization;

namespace API.Models;

public partial class User
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string? ProfileImageUrl { get; set; }

    public string? GoogleId { get; set; }

    public string? FacebookId { get; set; }

    public string? TwitterId { get; set; }

    public string? AppleId { get; set; }

    public string? RefreshToken { get; set; }

    public virtual ICollection<EventParticipant> EventParticipants { get; set; } = new List<EventParticipant>();

    [JsonIgnore]
    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
