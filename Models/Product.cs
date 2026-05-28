using System;
using System.Collections.Generic;
using System.IO;

namespace Shoes.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Article { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int MesurmentId { get; set; }

    public double Price { get; set; }

    public int SupplierId { get; set; }

    public int ManufacturerId { get; set; }

    public int ProductCategoryId { get; set; }

    public double Discount { get; set; }
    public bool IsHighDiscount => Discount > 15;
    public bool HasDiscount => Discount > 0;
    public double? FinalPrice => Price * (1.0 - Discount / 100.0);

    public int Count { get; set; }

    public string Description { get; set; } = null!;

    public string? photo;
    public string? Photo 
    {
        set => photo = value;
        get
        {
            if (string.IsNullOrWhiteSpace(photo))
            {
                return null;
            }
            else
            {
                return Path.Combine(Directory.GetCurrentDirectory(),"image", photo);
            }
                
        }
    }

    public virtual Manufacturer? Manufacturer { get; set; } = null!;

    public virtual Mesurment? Mesurment { get; set; } = null!;

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    public virtual ProductCategory? ProductCategory { get; set; } = null!;

    public virtual Supplier? Supplier { get; set; } = null!;
}
