using NewProductSaleApp.Models;

namespace NewProductSaleApp.Repositories
{
    public interface ISaleRepository
    {
        Task<IEnumerable<Sale>> GetAllSalesTransactionsAsync();
        Task<Sale> GetSaleByIdAsync(int id);
        Task<Sale> CreateSaleAsync(Sale sale);
    }
}
