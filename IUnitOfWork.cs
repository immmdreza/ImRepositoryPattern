using ImRepositoryPattern.Repository;
using Microsoft.EntityFrameworkCore;

namespace ImRepositoryPattern
{
    public interface IUnitOfWork<T> : IDisposable where T : DbContext, new()
    {
        public T Context { get; }

        /// <summary>
        /// Repository must have a constructor that takes only two params: <see cref="DbContext"/> and <see cref="UnitOfWork"/> 
        /// </summary>
        /// <param name="uniqueName">Unique tag for this repository</param>
        public void AddRepository<R, TContext, TEntity>(string uniqueName)
            where R : IRepository<TContext, TEntity>
            where TContext : DbContext, new() where TEntity : class;

        /// <summary>
        /// Repository must have a constructor that takes only two params: <see cref="DbContext"/> and <see cref="UnitOfWork"/> 
        /// </summary>
        /// <param name="uniqueName">Unique tag for this repository</param>
        public void AddRepository(string uniqueName, Type repositoryType);

        public R GetRepository<R>(string uniqueName);

        public IRepository<T, TEntity> GetBaseRepository<TContext, TEntity>()
            where TContext : DbContext, new() where TEntity : class;

        public Task<int> SaveAsync();
    }
}
