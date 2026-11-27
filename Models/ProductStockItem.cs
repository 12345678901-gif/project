using System;
using System.Collections.Generic;

namespace ООО_обувь.Models;

public partial class ProductStockItem
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int Count { get; set; }

    public virtual Product Product { get; set; } = null!;
}
