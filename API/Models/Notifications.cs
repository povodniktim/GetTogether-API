using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Notifications
{
    public int ID { get; set; }

    public int userID { get; set; }

    public int? eventID { get; set; }

    public int? participantID { get; set; }

    public string? status { get; set; }

    public virtual Events? _event { get; set; }

    public virtual EventParticipants? participant { get; set; }

    public virtual Users user { get; set; } = null!;
}
