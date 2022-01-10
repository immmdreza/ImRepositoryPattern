using ImRepositoryPattern.Repository;
using Microsoft.EntityFrameworkCore;

namespace ImRepositoryPattern
{
    public class ScopedUnitOfWork<T> : IDisposable,
        IUnitOfWork<T> where T : DbContext, new()
    {
        public T Context => _dbContext;

        private readonly Dictionary<string, Type> _repositories;
        private readonly Dictionary<string, object> _initializedRepositories;
        private readonly T _dbContext;

        public ScopedUnitOfWork(T dbContext)
        {
            _dbContext = dbContext;
            _repositories = new();
            _initializedRepositories = new();
        }

        /// <summary>
        /// Repository must have a constructor that takes only two params: <see cref="DbContext"/> and <see cref="UnitOfWork"/> 
        /// </summary>
        /// <param name="uniqueName">Unique tag for this repository</param>
        public void AddRepository<R, TContext, TEntity>(string uniqueName)
            where R : IRepository<TContext, TEntity>
            where TContext : DbContext, new() where TEntity : class
                => _repositories.Add(uniqueName, typeof(R));

        /// <summary>
        /// Repository must have a constructor that takes only two params: <see cref="DbContext"/> and <see cref="UnitOfWork"/> 
        /// </summary>
        /// <param name="uniqueName">Unique tag for this repository</param>
        public void AddRepository(string uniqueName, Type repositoryType)
            => _repositories.Add(uniqueName, repositoryType);

        public R GetRepository<R>(string uniqueName)
        {
            if (_initializedRepositories.ContainsKey(uniqueName))
            {
                return (R)_initializedRepositories[uniqueName];
            }
            else
            {
                var type = _repositories[uniqueName];
                var obj = Activator.CreateInstance(type, new object[] { Context, this });
                if (obj == null)
                    throw new InvalidOperationException($"Cant CreateInstance of {uniqueName}");

                _initializedRepositories.Add(uniqueName, obj);
                return (R)obj;
            }
        }

        public IRepository<T, TEntity> GetBaseRepository<TContext, TEntity>()
            where TContext : DbContext, new() where TEntity : class
                => new BaseRepository<T, TEntity>(Context, this);

        public async Task<int> SaveAsync()
        {
            return await Context.SaveChangesAsync().ConfigureAwait(false);
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                    _initializedRepositories.Clear();
                    _repositories.Clear();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
