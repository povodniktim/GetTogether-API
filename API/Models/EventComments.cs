using System;
using System.Collections.Generic;

namespace API.Models;

public partial class EventComments
{
    public int ID { get; set; }

    public int EventID { get; set; }

    public int UserID { get; set; }

    public string? Text { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Events Event { get; set; } = null!;

    public virtual Users User { get; set; } = null!;
}
