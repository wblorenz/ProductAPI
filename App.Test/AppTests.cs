using App.Database.Repositories;
using App.Model.Entities;
using App.Model.Exceptions;
using App.Model.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AspireApp.Server.Controllers;
using AspireApp.Server.DTOs;
using Testcontainers.PostgreSql;
using AppContext = App.Database.AppContext;

namespace App.Test
{
    [TestFixture]
    public class AppTests
    {
        private PostgreSqlContainer _postgresContainer = null!;
        private ServiceProvider _serviceProvider = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _postgresContainer = new PostgreSqlBuilder("postgres:18.3")
                .WithDatabase("testdb")
                .WithUsername("postgres")
                .WithPassword("postgres")
                .Build();

            await _postgresContainer.StartAsync();

            var services = new ServiceCollection();

            services.AddDbContext<AppContext>(options =>
            {
                options.UseNpgsql(_postgresContainer.GetConnectionString());
            });

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ProductController>();

            _serviceProvider = services.BuildServiceProvider();

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppContext>();
            await context.Database.MigrateAsync();
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            if (_serviceProvider != null)
            {
                await _serviceProvider.DisposeAsync();
            }

            if (_postgresContainer != null)
            {
                await _postgresContainer.DisposeAsync();
            }
        }

        [SetUp]
        public async Task SetUp()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppContext>();
            await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Products\" RESTART IDENTITY CASCADE;");
        }

        [Test]
        public async Task AddAndGetAsync_ShouldPersistProductAndRetrieveById()
        {
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IProductRepository>();

            var product = new Product
            {
                Name = "Mechanical Keyboard",
                Price = 129.99m,
                Description = "High quality RGB mechanical keyboard"
            };

            repository.Add(product);
            await repository.SaveAsync();

            Assert.That(product.Id, Is.GreaterThan(0));

            // Verify using a fresh DI scope to ensure data is retrieved from PostgreSQL
            using var readScope = _serviceProvider.CreateScope();
            var readRepo = readScope.ServiceProvider.GetRequiredService<IProductRepository>();
            var retrieved = await readRepo.GetAsync(product.Id);

            Assert.That(retrieved, Is.Not.Null);
            Assert.That(retrieved!.Name, Is.EqualTo("Mechanical Keyboard"));
            Assert.That(retrieved.Price, Is.EqualTo(129.99m));
            Assert.That(retrieved.Description, Is.EqualTo("High quality RGB mechanical keyboard"));
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllProducts()
        {
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IProductRepository>();

            repository.Add(new Product
            {
                Name = "Gaming Mouse",
                Price = 59.99m,
                Description = "High-precision ergonomic optical gaming mouse"
            });
            repository.Add(new Product
            {
                Name = "USB-C Headset",
                Price = 89.90m,
                Description = "Noise-cancelling over-ear USB-C gaming headset"
            });
            await repository.SaveAsync();

            using var readScope = _serviceProvider.CreateScope();
            var readRepo = readScope.ServiceProvider.GetRequiredService<IProductRepository>();
            var products = await readRepo.GetAllAsync();

            Assert.That(products, Has.Count.EqualTo(2));
            Assert.That(products.Select(p => p.Name), Does.Contain("Gaming Mouse").And.Contain("USB-C Headset"));
        }

        [Test]
        public async Task GetAsync_WithNonExistentId_ShouldReturnNull()
        {
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IProductRepository>();

            var nonExistent = await repository.GetAsync(99999);

            Assert.That(nonExistent, Is.Null);
        }

        [Test]
        public async Task Delete_ShouldRemoveProductFromDatabase()
        {
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IProductRepository>();

            var product = new Product
            {
                Name = "Desk Mat XXL",
                Price = 29.99m,
                Description = "Extended waterproof non-slip rubber desk mat"
            };

            repository.Add(product);
            await repository.SaveAsync();

            // Delete product in a new scope
            using var deleteScope = _serviceProvider.CreateScope();
            var deleteRepo = deleteScope.ServiceProvider.GetRequiredService<IProductRepository>();
            var toDelete = await deleteRepo.GetAsync(product.Id);
            Assert.That(toDelete, Is.Not.Null);

            deleteRepo.Delete(toDelete!);
            await deleteRepo.SaveAsync();

            // Verify deletion in another scope
            using var verifyScope = _serviceProvider.CreateScope();
            var verifyRepo = verifyScope.ServiceProvider.GetRequiredService<IProductRepository>();
            var deletedProduct = await verifyRepo.GetAsync(product.Id);

            Assert.That(deletedProduct, Is.Null);
        }

        [Test]
        public void SaveAsync_WithInvalidProduct_ShouldThrowDomainValidationException()
        {
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IProductRepository>();

            var invalidProduct = new Product
            {
                Name = "A",           // Less than 3 characters
                Price = -10.0m,       // Negative price
                Description = "Short" // Less than 10 characters
            };

            repository.Add(invalidProduct);

            var ex = Assert.ThrowsAsync<DomainValidationException>(async () => await repository.SaveAsync());
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex!.Validators.Count(), Is.EqualTo(3));
        }

        [Test]
        public async Task PostProduct_WithValidData_ShouldCreateProductSuccessfully()
        {
            using var scope = _serviceProvider.CreateScope();
            var controller = scope.ServiceProvider.GetRequiredService<ProductController>();

            var newProductDto = new ProductDTO
            {
                Name = "Wireless Headset",
                Price = 89.99m,
                Description = "High quality wireless noise cancelling headset"
            };

            var createdProduct = await controller.NewProduct(newProductDto);

            Assert.That(createdProduct, Is.Not.Null);
            Assert.That(createdProduct.Id, Is.GreaterThan(0));
            Assert.That(createdProduct.Name, Is.EqualTo("Wireless Headset"));
            Assert.That(createdProduct.Price, Is.EqualTo(89.99m));
            Assert.That(createdProduct.Description, Is.EqualTo("High quality wireless noise cancelling headset"));

            // Verify product is persisted in the database
            using var verifyScope = _serviceProvider.CreateScope();
            var verifyRepo = verifyScope.ServiceProvider.GetRequiredService<IProductRepository>();
            var persisted = await verifyRepo.GetAsync(createdProduct.Id);

            Assert.That(persisted, Is.Not.Null);
            Assert.That(persisted!.Name, Is.EqualTo("Wireless Headset"));
            Assert.That(persisted.Price, Is.EqualTo(89.99m));
            Assert.That(persisted.Description, Is.EqualTo("High quality wireless noise cancelling headset"));
        }

        [Test]
        public void PostProduct_WithInvalidData_ShouldFailValidation()
        {
            using var scope = _serviceProvider.CreateScope();
            var controller = scope.ServiceProvider.GetRequiredService<ProductController>();

            var invalidProductDto = new ProductDTO
            {
                Name = "A",           // Invalid: less than 3 characters
                Price = -15.0m,       // Invalid: negative price
                Description = "Short" // Invalid: less than 10 characters
            };

            var ex = Assert.ThrowsAsync<DomainValidationException>(async () =>
                await controller.NewProduct(invalidProductDto));

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex!.Validators, Is.Not.Empty);
            Assert.That(ex.Validators.Count(), Is.EqualTo(3));
        }
    }
}
