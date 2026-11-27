using System;
using System.Collections.Generic;

namespace ООО_обувь.Models;

public partial class Supplier
{
    public string Name { get; set; } = null!;

    public int Id { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
