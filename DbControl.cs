using Microsoft.EntityFrameworkCore;
using Shoes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Shoes
{
    public static class DbControl
    {
        public static User? GetAuthUser(string email, string password)
        {
            using (ShoesContext ctx = new ShoesContext())
            {
                return ctx.Users.FirstOrDefault(e => e.Email == email && e.Password == password);
            }
        }
        public static List<Product> GetProducts(string? searchStr = null, bool? descSort = false, Supplier? filter = null)
        {
            using (ShoesContext ctx = new ShoesContext())
            {
                var query = ctx.Products
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Supplier)
                    .Include(p => p.Mesurment)
                    .Include(p => p.ProductCategory)
                    .AsQueryable();

                if (descSort is true)
                {
                    query = query.OrderByDescending(p => p.Count);
                }
                else
                {
                    query = query.OrderBy(p => p.Count);
                }

                if (filter != null && filter.Id != 0)
                {
                    query = query.Where(p => p.Supplier.Id == filter.Id);
                }

                if (string.IsNullOrWhiteSpace(searchStr))
                {
                    return query.ToList();
                }

                var words = searchStr.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (var word in words)
                {
                    var s = word.ToLower();
                    query = query.Where(p =>
                    p.Name.ToLower().Contains(s) ||
                    p.Description.ToLower().Contains(s) ||
                    p.Manufacturer.Name.ToLower().Contains(s) ||
                    p.Supplier.Name.ToLower().Contains(s) ||
                    p.Mesurment.Name.ToLower().Contains(s) ||
                    p.ProductCategory.Name.ToLower().Contains(s));
                }
                return query.ToList();
            }
        }

        public static List<Supplier> GetSuppliers()
        {
            using (ShoesContext ctx = new ShoesContext())
            {
                return ctx.Suppliers.ToList();
            }
        }

        public static List<Manufacturer> GetManufacturers()
        {
            using (ShoesContext ctx = new ShoesContext())
            {
                return ctx.Manufacturers.ToList();
            }
        }

        public static List<ProductCategory> GetProductCategories()
        {
            using (ShoesContext ctx = new ShoesContext())
            {
                return ctx.ProductCategories.ToList();
            }
        }

        public static List<Mesurment> GetMesurmentrs()
        {
            using (ShoesContext ctx = new ShoesContext())
            {
                return ctx.Mesurments.ToList();
            }
        }

        public static List<Order> GetOrders()
        {
            using (ShoesContext ctx = new ShoesContext())
            {
                return ctx.Orders
                    .Include(p => p.Addres)
                    .Include(p => p.OrderStatus)
                    .Include(p => p.User)
                    .Include(p => p.OrderProducts)
                    .ToList();
            }
        }

        public static List<OrderStatus> GetOrderStatuses()
        {
            using (ShoesContext ctx = new ShoesContext())
            {
                return ctx.OrderStatuses.ToList();
            }
        }

        public static List<PickupPoint> GetPickupPoints()
        {
            using (ShoesContext ctx = new ShoesContext())
            {
                return ctx.PickupPoints.ToList();
            }
        }

        public static List<OrderProduct> GetOrdersProducts()
        {
            using (ShoesContext ctx = new ShoesContext())
            {
                return ctx.OrderProducts
                    .Include(p => p.Product)
                    .Include(p => p.Order)
                    .ThenInclude(o => o.OrderStatus)
                    .Include(p => p.Order)
                    .ThenInclude(p => p.Addres)
                    .ToList();
            }
        }

        public static void AddProduct(Product product)
        {
            using (ShoesContext ctx = new ShoesContext())
            {
                ctx.Products.Add(product);
                ctx.SaveChanges();
            }
        }

        public static void UpdateProduct(Product product)
        {
            using (ShoesContext ctx = new ShoesContext())
            {
                ctx.Products.Update(product);
                ctx.SaveChanges();
            }
        }

        public static void DeleteProduct(Product product)
        {
            using (ShoesContext ctx = new ShoesContext())
            {
                ctx.Products.Remove(product);
                ctx.SaveChanges();
            }
        }

        public static void AddOrder(Order order)
        {
            using (ShoesContext ctx = new ShoesContext())
            {
                ctx.Orders.Add(order);
                ctx.SaveChanges();
            }
        }

        public static void UpdateOrder(Order order)
        {
            using (ShoesContext ctx = new ShoesContext())
            {
                var dbOrder = ctx.Orders.FirstOrDefault(o => o.Id == order.Id);
                if (dbOrder == null) return;

                dbOrder.OrdrerDate = order.OrdrerDate;
                dbOrder.DeliveryDate = order.DeliveryDate;
                dbOrder.AddresId = order.AddresId;
                dbOrder.UserId = order.UserId;
                dbOrder.Code = order.Code;
                dbOrder.OrderStatusId = order.OrderStatusId;

                ctx.SaveChanges();
            }
        }

        public static void DeleteOrder(Order order)
        {
            using (ShoesContext ctx = new ShoesContext())
            {
                ctx.Orders.Remove(order);
                ctx.SaveChanges();
            }
        }

        public static void AddOrderProduct(OrderProduct orderProduct)
        {
            using (ShoesContext ctx = new ShoesContext())
            {
                var entity = new OrderProduct
                {
                    Id = orderProduct.Id,
                    OrderId = orderProduct.OrderId,
                    ProductId = orderProduct.ProductId,
                    Count = orderProduct.Count
                };

                ctx.OrderProducts.Attach(entity);
                ctx.Entry(entity).State = EntityState.Modified;  
                ctx.SaveChanges();
            }
        }

        public static void UpdateOrderProduct(OrderProduct orderProduct)
        {
            using (ShoesContext ctx = new ShoesContext())
            {
                var dbOrderProduct = ctx.OrderProducts.FirstOrDefault(op => op.Id == orderProduct.Id);
                if (dbOrderProduct == null) return;

                dbOrderProduct.OrderId = orderProduct.OrderId;
                dbOrderProduct.ProductId = orderProduct.ProductId;
                dbOrderProduct.Count = orderProduct.Count;

                ctx.SaveChanges();
            }
        }

        public static void DeleteOrderProduct(OrderProduct orderProduct)
        {
            using (ShoesContext ctx = new ShoesContext())
            {
                ctx.OrderProducts.Remove(orderProduct);
                ctx.SaveChanges();
            }
        }

        public static bool HasLinkedOrders(Product product)
        {
            using (ShoesContext ctx = new ShoesContext())
            {
                return ctx.OrderProducts.Any(op => op.ProductId == product.Id);
            }
        }

    }
}
