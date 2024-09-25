using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewProductSaleApp.Models;
using NewProductSaleApp.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewProductSaleApp.Controllers
{
    [Route("api")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly ISaleRepository _saleRepository;

        public SalesController(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        // GET: api/saletransactions
        [HttpGet("saletransactions")]
        public async Task<ActionResult<IEnumerable<Sale>>> GetSalesTransactions()
        {
            var sales = await _saleRepository.GetAllSalesTransactionsAsync();
            return Ok(sales);
        }

        // GET: api/salebyid/{id}
        [HttpGet("salebyid/{id}")]
        public async Task<ActionResult<Sale>> GetSaleById(int id)
        {
            var sale = await _saleRepository.GetSaleByIdAsync(id);
            if (sale == null)
            {
                return NotFound();
            }
            return Ok(sale);
        }

        // POST: api/sale
        [HttpPost("sale")]
        public async Task<ActionResult<Sale>> PostSale(Sale sale)
        {
            if (sale.SaleTransactions == null || !sale.SaleTransactions.Any())
            {
                return BadRequest("SaleTransactions cannot be empty.");
            }

            try
            {
                var createdSale = await _saleRepository.CreateSaleAsync(sale);
                return CreatedAtAction(nameof(GetSaleById), new { id = createdSale.Id }, createdSale);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
