using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Primex.Models;

public partial class Order
{
    
    public int IdOrders { get; set; }
    
    public int IdUser { get; set; }
   
    public int IdService { get; set; }
  
    public int Price { get; set; }
    
    public string Status { get; set; } = null!;
  
    public DateOnly Date { get; set; }
    
    public TimeOnly Time { get; set; }
   
    public string Address { get; set; } = null!;

    public virtual Service? IdServiceNavigation { get; set; }
    public virtual User? IdUserNavigation { get; set; }

}
