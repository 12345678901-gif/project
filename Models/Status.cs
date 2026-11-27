using System;
using System.Collections.Generic;

namespace ООО_обувь.Models;

public partial class Status
{
    public string Name { get; set; } = null!;

    public int Id { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
