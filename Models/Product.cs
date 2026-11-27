using System;
using System.Collections.Generic;

namespace ООО_обувь.Models;

public partial class Product
{
    public int Id { get; set; }

    public string? Article { get; set; }

    public string Name { get; set; } = null!;

    public int UnitsId { get; set; }

    public decimal Price { get; set; }

    public int SupplierId { get; set; }

    public int ManufacturersId { get; set; }

    public int CategoriesId { get; set; }

    public decimal Sale { get; set; }

    public string? Description { get; set; }

    public string? Image { get; set; }

    public virtual Category Categories { get; set; } = null!;

    public virtual Manufacturer Manufacturers { get; set; } = null!;

    public virtual ICollection<OrdersProduct> OrdersProducts { get; set; } = new List<OrdersProduct>();

    public virtual ProductStockItem? ProductStockItem { get; set; }

    public virtual Supplier Supplier { get; set; } = null!;

    public virtual Unit Units { get; set; } = null!;
}
