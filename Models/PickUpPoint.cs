using System;
using System.Collections.Generic;

namespace ООО_обувь.Models;

public partial class PickUpPoint
{
    public int Id { get; set; }

    public int Index { get; set; }

    public int StreetId { get; set; }

    public int? House { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Street Street { get; set; } = null!;
}
