using System;
using System.Collections.Generic;
using System.Text;

namespace App.Database.Repositories
{
    public abstract class BaseRepository<TEntity>(AppContext context) : App.Model.Repositories.IBaseRepository<TEntity>
    {
        public abstract void Add(TEntity entity);
        public abstract void Delete(TEntity entity);
        public abstract Task<List<TEntity>> GetAllAsync();
        public abstract Task<TEntity?> GetAsync(long id);

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
