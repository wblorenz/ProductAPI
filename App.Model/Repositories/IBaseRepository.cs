using System;
using System.Collections.Generic;
using System.Text;

namespace App.Model.Repositories
{
    public interface IBaseRepository<TEntity>
    {
        Task<List<TEntity>> GetAllAsync();
        Task<TEntity?> GetAsync(long id);
        void Add(TEntity entity);
        void Delete(TEntity entity);
        Task SaveAsync();
    }
}
