using App.Model;
using AspireApp.Server.DTOs;
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
        public async Task<IEnumerable<ProductDTO>> GetProducts()
        {
            var products = await _context.Products.ToListAsync();
            return products.Select(p => new ProductDTO(p));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(long id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return new ProductDTO(product);
        }

        [HttpPost]
        public async Task<ProductDTO> NewProduct([FromBody] ProductDTO product)
        {
            var newProduct = product.ToProduct();
            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();
            return new ProductDTO(newProduct);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<ProductDTO>> UpdateProduct(long id, [FromBody] ProductDTO product)
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
