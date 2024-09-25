using Microsoft.EntityFrameworkCore;
using NewProductSaleApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewProductSaleApp.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly SalesDbContext _context;
        public SaleRepository(SalesDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Sale>> GetAllSalesTransactionsAsync()
        {
            return await _context.Sales
                .Include(s => s.SaleTransactions)
                .ThenInclude(st => st.Product)
                .ToListAsync();
        }
        public async Task<Sale> GetSaleByIdAsync(int id)
        {
            return await _context.Sales
                .Include(s => s.SaleTransactions)
                .ThenInclude(st => st.Product)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
        public async Task<Sale> CreateSaleAsync(Sale sale)
        {
            foreach (var transaction in sale.SaleTransactions)
            {
                var product = await _context.Products.FindAsync(transaction.ProductId);
                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with ID {transaction.ProductId} not found.");
                }

                transaction.Amount = product.Price * transaction.Quantity;
                transaction.Tax = CalculateTax(product, transaction.Amount);
                transaction.ImportDuty = product.IsImported ? CalculateImportDuty(transaction.Amount) : 0;
            }

            sale.NetAmount = sale.SaleTransactions.Sum(st => st.Amount);
            sale.TotalTaxAmount = sale.SaleTransactions.Sum(st => st.Tax);
            sale.TotalImportDuty = sale.SaleTransactions.Sum(st => st.ImportDuty);
            sale.TotalAmount = sale.NetAmount + sale.TotalTaxAmount + sale.TotalImportDuty;
            sale.SaleDate = DateTime.UtcNow;
            sale.InvoiceNumber = GenerateInvoiceNumber();

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            return sale;
        }
        private decimal CalculateTax(Product product, decimal amount)
        {
            if (product.ProductType == "essential") return 0;
            return Math.Ceiling((10 * amount) / 100 / 0.05m) * 0.05m;
        }

        private decimal CalculateImportDuty(decimal amount)
        {
            return Math.Ceiling((5 * amount) / 100 / 0.05m) * 0.05m;
        }

        private string GenerateInvoiceNumber()
        {
            var latestSale = _context.Sales.OrderByDescending(s => s.Id).FirstOrDefault();
            int newInvoiceNumber = (latestSale?.Id ?? 0) + 1;
            return $"INV{newInvoiceNumber:D6}";
        }
    }
}
