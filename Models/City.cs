using System;
using System.Collections.Generic;

namespace ООО_обувь.Models;

public partial class City
{
    public string Name { get; set; } = null!;

    public int Id { get; set; }

    public virtual ICollection<Street> Streets { get; set; } = new List<Street>();
}
