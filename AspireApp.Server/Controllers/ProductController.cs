using App.Model;
using Microsoft.AspNetCore.Mvc;

namespace AspireApp.Server.Controllers
{
    [ApiController]                     // 1. Enables API-specific behaviors (like automatic 400 validation)
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<Product> GetProducts()
        {
            // For demonstration purposes, return a dummy product.
            return new List<Product>
            {
                new Product
                {
                    Id = 3290L,
                    Name = "Sample Product",
                    Price = 19.99m,
                    Description = "This is a sample product."
                }
            };
        }
        [HttpGet("{id}")]
        public Product GetProduct(long id)
        {
            // For demonstration purposes, return a dummy product.
            return new Product
            {
                Id = id,
                Name = "Sample Product",
                Price = 19.99m,
                Description = "This is a sample product."
            };
        }
        [HttpPost]
        public Product NewProduct([FromBody] Product product)
        {
            // For demonstration purposes, return the same product.
            return product;
        }


        [HttpPut("{id}")]
        public Product UpdateProduct(long id, [FromBody] Product product)
        {
            // For demonstration purposes, return the same product.
            return product;
        }
        [HttpDelete("{id}")]
        public void DeleteProduct(long id)
        {
            // For demonstration purposes, do nothing.
        }
    }
}
