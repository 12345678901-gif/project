using System;
using System.Collections.Generic;

namespace ООО_обувь.Models;

public partial class Street
{
    public int CityId { get; set; }

    public string? Name { get; set; }

    public int Id { get; set; }

    public virtual City City { get; set; } = null!;

    public virtual ICollection<PickUpPoint> PickUpPoints { get; set; } = new List<PickUpPoint>();
}
