using App.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspireApp.Server.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly App.Database.AppContext _context;

        public ProductController(App.Database.AppContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(long id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return product;
        }

        [HttpPost]
        public async Task<Product> NewProduct([FromBody] Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> UpdateProduct(long id, [FromBody] Product product)
        {
            var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (existingProduct == null)
            {
                return NotFound();
            }
            existingProduct.Description = product.Description;
            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;
            await _context.SaveChangesAsync();
            return product;
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(long id)
        {
            var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (existingProduct == null)
            {
                return NotFound();
            }
            _context.Products.Remove(existingProduct);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
