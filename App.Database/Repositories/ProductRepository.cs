using App.Model.Entities;
using App.Model.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Database.Repositories
{
    public class ProductRepository(AppContext context) : BaseRepository<Product>(context), IProductRepository
    {
        public override void Add(Product entity)
        {
            context.Products.Add(entity);
        }

        public override void Delete(Product entity)
        {
            context.Products.Remove(entity);
        }

        public override async Task<Product?> GetAsync(long id)
        {
            return await context.Products.FirstOrDefaultAsync(x => x.Id == id);
        }

        public override async Task<List<Product>> GetAllAsync()
        {
            return await context.Products.ToListAsync();
        }
    }
}
