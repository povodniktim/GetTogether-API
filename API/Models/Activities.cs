using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Activities
{
    public int ID { get; set; }

    public string name { get; set; } = null!;

    public virtual ICollection<Events> Events { get; set; } = new List<Events>();
}
