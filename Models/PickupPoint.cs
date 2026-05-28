using System;
using System.Collections.Generic;

namespace Shoes.Models;

public partial class PickupPoint
{
    public int Id { get; set; }

    public string Town { get; set; } = null!;

    public string Street { get; set; } = null!;

    public string House { get; set; } = null!;

    public string FullAddress => $"{Town}, ул. {Street}, д. {House}, {Article}";

    public int Article { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
