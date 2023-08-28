using System;
using System.Collections.Generic;

namespace API.Models;

public partial class EventPhotos
{
    public int ID { get; set; }

    public int EventID { get; set; }

    public string URL { get; set; } = null!;

    public DateTime UploadedAt { get; set; }

    public virtual Events Event { get; set; } = null!;
}
