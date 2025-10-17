using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataServiceLayer.Entities;
using DataServiceLayer.Dtos;

namespace DataServiceLayer
{
    public class DataService
    {
        private NorthwindContext ctx;

        public DataService()
        {
            ctx = new NorthwindContext();
        }


        // Categories

        public List<Category> GetCategories()
        {
            return ctx.Categories
                .AsNoTracking() //we dont want to track changes on our list
                .OrderBy(c => c.Name)
                .ToList();
        }

        public Category GetCategory(int id)
        {
            return ctx.Categories.Where(c => c.Id == id).SingleOrDefault();
            // Alternatively, ctx.Categories.Find(id) works fine in this particular case. However, it is inconsistent with the way queries are structured via LINQ as Find() is only applicable when the result of any prior LINQ query is a DbSet (or potentially similar). In essence, it locks us out of using LINQ for a lot of stuff, so it is worth being aware that there are other ways of getting the same thing. Additionally, there might be an argument in terms of getting the whole collection and then finding stuf in that, being more costly. I have no idea what happens under the hood specifically, so its hard to be sure. But it would seem intuitive that the LINQ query returns the whole set, and then we search that, which is not how a relational DB is intended to be used (AFAIK)
            // This is additionally a great example of where to use .SingleOrDefault as we want to return null in case it is not found.
        }

        public Category CreateCategory(string name, string desc)
        {

            int nextId = ctx.Categories.OrderByDescending(c => c.Id).First().Id + 1; //the highest id +1
            Category target = new Category { Id = nextId, Name = name, Description = desc };
            ctx.Categories.Add(target);

            try
            {
                ctx.SaveChanges(); //try to save
                return target;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool DeleteCategory(int id)
        {
            try
            {
                Category target = ctx.Categories.Where(c => c.Id == id).Single();
                ctx.Categories.Remove(target);
                return ctx.SaveChanges() > 0; //if more than 0 changes, we assume it worked
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateCategory(int id, string name, string desc)
        {
            try
            {
                Category target = ctx.Categories.Where(c => c.Id == id).Single();
                target.Name = name;
                target.Description = desc;
                return ctx.SaveChanges() > 0; //if more than 0 changes, we assume it worked
            }
            catch (Exception)
            {
                return false;
            }

        }

        // Products

        public Product GetProduct(int id)
        {
            return ctx.Products.Where(p => p.Id == id).Include(p => p.Category).Single();
            // So, instead of .Find(id) which is a method on the DbSet<T> class that is uesfull if we just whant the T (generic class) as that actual object. However, if we need to bring along object nested from references, this limits us as .Find() is not (AFAIK) a part of LINQ, whereby it can only be used if we have a DbSet and want a particular element by primary id.
        }

        public List<ProductByCategory> GetProductByCategory(int catId)
        {
            return ctx.Products
                .AsNoTracking()
                .Where(p => p.CategoryId == catId)
                .Include(p => p.Category)
                .Select(p => new ProductByCategory(p.Id, p.Name, p.UnitPrice, p.QuantityPerUnit, p.UnitsInStock, p.Category.Name))
                .ToList();
        }

        public List<ProductByName> GetProductByName(string substring)
        {
            return ctx.Products.Include(p => p.Category)
                .Where(p => p.Name.Contains(substring) == true) //order matters as Where clause does not work on the DTO unless it is in a DbSet<DTO>
                .Select(p => new ProductByName(p.Name, p.Category.Name))
                .ToList();
        }

        // Orders

        public Order GetOrder(int id)
        {
            return ctx.Orders.Where(o => o.Id == id).Include(o => o.OrderDetails).ThenInclude(d => d.Product).ThenInclude(p => p.Category).SingleOrDefault();
        }

        public List<Order> GetOrders()
        {
            return ctx.Orders.ToList();
        }

        public List<Order> GetOrders(string substring)
        {
            return ctx.Orders.Where(p => p.ShipName.Contains(substring) == true).ToList();
        }

        // OrderDetails

        public List<OrderDetails> GetOrderDetailsByOrderId(int id)
        {
            return ctx.OrderDetails.Where(d => d.Order.Id == id).Include(d => d.Product).Include(d => d.Order).ToList(); //multiple include since we want both (instead of thenInclude)
        }

        public List<OrderDetails> GetOrderDetailsByProductId(int id)
        {
            return ctx.OrderDetails.Where(d => d.Product.Id == id).Include(d => d.Product).Include(d => d.Order).ToList(); //multiple include since we want both (instead of thenInclude)
        }
    }

    // EF northwind context

    public class NorthwindContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseNpgsql("host=localhost;db=northwind;uid=postgres;pwd=Faqvy0-ruvxan-tigkac");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().ToTable("categories");
            modelBuilder.Entity<Category>().Property(x => x.Id).HasColumnName("categoryid");
            modelBuilder.Entity<Category>().Property(x => x.Name).HasColumnName("categoryname");
            modelBuilder.Entity<Category>().Property(x => x.Description).HasColumnName("description");

            modelBuilder.Entity<Product>().ToTable("products");
            modelBuilder.Entity<Product>().Property(x => x.Id).HasColumnName("productid");
            modelBuilder.Entity<Product>().Property(x => x.Name).HasColumnName("productname");
            modelBuilder.Entity<Product>().Property(x => x.UnitPrice).HasColumnName("unitprice");
            modelBuilder.Entity<Product>().Property(x => x.QuantityPerUnit).HasColumnName("quantityperunit");
            modelBuilder.Entity<Product>().Property(x => x.UnitsInStock).HasColumnName("unitsinstock");
            modelBuilder.Entity<Product>().Property(x => x.CategoryId).HasColumnName("categoryid");
            modelBuilder.Entity<Product>().Navigation(p => p.Category);

            modelBuilder.Entity<Order>().ToTable("orders");
            modelBuilder.Entity<Order>().Property(x => x.Id).HasColumnName("orderid");
            modelBuilder.Entity<Order>().Property(x => x.Date).HasColumnName("orderdate");
            modelBuilder.Entity<Order>().Property(x => x.Required).HasColumnName("requireddate");
            modelBuilder.Entity<Order>().Property(x => x.Shipped).HasColumnName("shippeddate");
            modelBuilder.Entity<Order>().Property(x => x.Freight).HasColumnName("freight");
            modelBuilder.Entity<Order>().Property(x => x.ShipName).HasColumnName("shipname");
            modelBuilder.Entity<Order>().Property(x => x.ShipCity).HasColumnName("shipcity");
            modelBuilder.Entity<Order>().Navigation(o => o.OrderDetails);


            modelBuilder.Entity<OrderDetails>().ToTable("orderdetails");
            modelBuilder.Entity<OrderDetails>().Property(x => x.OrderId).HasColumnName("orderid");
            modelBuilder.Entity<OrderDetails>().Property(x => x.ProductId).HasColumnName("productid");
            modelBuilder.Entity<OrderDetails>().HasKey(x => new { x.OrderId, x.ProductId });
            modelBuilder.Entity<OrderDetails>().Property(x => x.UnitPrice).HasColumnName("unitprice");
            modelBuilder.Entity<OrderDetails>().Property(x => x.Quantity).HasColumnName("quantity");
            modelBuilder.Entity<OrderDetails>().Property(x => x.Discount).HasColumnName("discount");
            modelBuilder.Entity<OrderDetails>().Navigation(d => d.Order);
            modelBuilder.Entity<OrderDetails>().Navigation(d => d.Product);
        }
    }
}
