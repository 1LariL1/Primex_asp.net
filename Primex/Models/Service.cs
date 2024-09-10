using System;
using System.Collections.Generic;

namespace Primex.Models;

public partial class Service
{
    public int IdService { get; set; }

    public string Service1 { get; set; } = null!;

    public int Price { get; set; }

    public string Description { get; set; } = null!;

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
