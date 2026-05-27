using Microsoft.EntityFrameworkCore;
using Shoes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
