using System.Text.Json.Serialization;

namespace API.Models;

public partial class UserActivity
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ActivityId { get; set; }

    [JsonIgnore]
    public virtual Activity Activity { get; set; } = null!;

    [JsonIgnore]
    public virtual User User { get; set; } = null!;
}
