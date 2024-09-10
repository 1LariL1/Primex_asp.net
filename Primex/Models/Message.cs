using System;
using System.Collections.Generic;

namespace Primex.Models;

public partial class Message
{
    public int IdMessage { get; set; }

    public int IdService { get; set; }

    public int IdUser { get; set; }

    public string Address { get; set; } = null!;

    public string Comment { get; set; } = null!;

    public string Time { get; set; } = null!;

    public DateOnly Date { get; set; }

    public virtual Service? IdServiceNavigation { get; set; }

    public virtual User? IdUserNavigation { get; set; } 
}
