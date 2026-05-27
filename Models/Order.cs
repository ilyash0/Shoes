using System;
using System.Collections.Generic;

namespace Shoes.Models;

public partial class Order
{
    public int Id { get; set; }

    public DateOnly OrdrerDate { get; set; }

    public DateOnly DeliveryDate { get; set; }

    public int AddresId { get; set; }

    public int UserId { get; set; }

    public int Code { get; set; }

    public int OrderStatusId { get; set; }

    public virtual PickupPoint Addres { get; set; } = null!;

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    public virtual OrderStatus OrderStatus { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
