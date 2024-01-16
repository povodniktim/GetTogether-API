namespace API.Models;

public partial class Activity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string IconClassName { get; set; } = null!;

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<UserActivity> UserActivities { get; set; } = new List<UserActivity>();
}
