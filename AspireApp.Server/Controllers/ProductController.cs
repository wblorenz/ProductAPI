using App.Model.Repositories;
using AspireApp.Server.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AspireApp.Server.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController(IProductRepository productRepository) : ControllerBase
    {

        [HttpGet]
        public async Task<IEnumerable<ProductDTO>> GetProducts()
        {
            var products = await productRepository.GetAllAsync();
            return products.Select(p => new ProductDTO(p));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(long id)
        {
            var product = await productRepository.GetAsync(id);
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
            productRepository.Add(newProduct);
            await productRepository.SaveAsync();
            return new ProductDTO(newProduct);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProductDTO>> UpdateProduct(long id, [FromBody] ProductDTO product)
        {
            var existingProduct = await productRepository.GetAsync(id);
            if (existingProduct == null)
            {
                return NotFound();
            }
            existingProduct.Description = product.Description;
            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;
            await productRepository.SaveAsync();
            product.Id = existingProduct.Id;
            return product;
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(long id)
        {
            var existingProduct = await productRepository.GetAsync(id);
            if (existingProduct == null)
            {
                return NotFound();
            }
            productRepository.Delete(existingProduct);
            await productRepository.SaveAsync();
            return NoContent();
        }
    }
}
