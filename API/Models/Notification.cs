﻿namespace API.Models;

public partial class Notification
{
    public int Id { get; set; }

    public int OrganizerId { get; set; }

    public int? EventId { get; set; }

    public int? ParticipantId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? Status { get; set; }

    public virtual Event? Event { get; set; }

    public virtual EventParticipant? Participant { get; set; }

    public virtual User Organizer { get; set; } = null!;
}
