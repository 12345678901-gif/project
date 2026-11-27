using System;
using System.Collections.Generic;

namespace ООО_обувь.Models;

public partial class Order
{
    public int Id { get; set; }

    public DateOnly? ДатаЗаказа { get; set; }

    public DateOnly? ДатаДоставки { get; set; }

    public int АдресПунктаВыдачи { get; set; }

    public int UserId { get; set; }

    public int КодДляПолучения { get; set; }

    public int StatusId { get; set; }

    public virtual ICollection<OrdersProduct> OrdersProducts { get; set; } = new List<OrdersProduct>();

    public virtual Status Status { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual PickUpPoint АдресПунктаВыдачиNavigation { get; set; } = null!;
}
