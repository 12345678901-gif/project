using System;
using System.Collections.Generic;

namespace ООО_обувь.Models;

public partial class OrdersProduct
{
    public int Id { get; set; }

    public int OrdersId { get; set; }

    public int ProductId { get; set; }

    public int Count { get; set; }

    public decimal? Price { get; set; }

    public virtual Order Orders { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
