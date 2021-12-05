using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Birdie.Data.Repositories
{
    internal abstract class RepositoryBase<T> where T : class, new()
    {
        private readonly BirdieDbContext birdieDbContext;

        public RepositoryBase(BirdieDbContext birdieDbContext)
        {
            this.birdieDbContext = birdieDbContext;
        }

        protected virtual IQueryable<T> Get(Func<T, bool> where = null)
        {
            if (where == null) return birdieDbContext.Set<T>().AsQueryable();

            return birdieDbContext.Set<T>().Where(where).AsQueryable();
        }

        protected virtual async Task<T> Create(T entity)
        {
            await birdieDbContext.Set<T>().AddAsync(entity);
            await birdieDbContext.SaveChangesAsync();
            return entity;
        }

        protected virtual async Task<T> Update(T entity)
        {
            EntityEntry<T> entry = birdieDbContext.Entry<T>(entity);
            entry.State = EntityState.Modified;
            await birdieDbContext.SaveChangesAsync();
            return entity;
        }
    }
}
