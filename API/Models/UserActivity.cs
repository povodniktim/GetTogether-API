using System;
using System.Collections.Generic;

namespace API.Models;

public partial class UserActivity
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ActivityId { get; set; }

    public virtual Activity Activity { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
