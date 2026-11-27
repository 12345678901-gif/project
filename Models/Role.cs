using System;
using System.Collections.Generic;

namespace ООО_обувь.Models;

public partial class Role
{
    public string Name { get; set; } = null!;

    public int Id { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
