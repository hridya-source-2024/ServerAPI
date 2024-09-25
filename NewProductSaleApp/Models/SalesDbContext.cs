using Microsoft.EntityFrameworkCore;

namespace NewProductSaleApp.Models
{
    public class SalesDbContext : DbContext
    {
        public SalesDbContext(DbContextOptions<SalesDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Sale> Sales { get; set; }

        public DbSet<SaleTransaction> SaleTransactions { get; set; }

    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int TaxRate { get; set; }
        public string ProductType { get; set; } 
        public bool IsImported { get; set; }
        public decimal ImportDuty { get; set; }
    }

    public class Sale
    {
        public int Id { get; set; }
        public DateTime SaleDate { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public decimal NetAmount { get; set; }
        public decimal TotalTaxAmount { get; set; }
        public decimal TotalImportDuty { get; set; }
        public decimal TotalAmount { get; set; }

        public virtual ICollection<SaleTransaction> SaleTransactions { get; set; } = new List<SaleTransaction>();
    }

    public class SaleTransaction
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int SaleId { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public decimal Tax { get; set; }
        public decimal ImportDuty { get; set; }
    }

}
