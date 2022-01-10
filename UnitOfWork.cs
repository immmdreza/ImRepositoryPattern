using ImRepositoryPattern.Repository;
using Microsoft.EntityFrameworkCore;

namespace ImRepositoryPattern
{
    public class UnitOfWork<T> : IDisposable,
        IUnitOfWork<T> where T : DbContext, new()
    {
        public T Context { get; }

        private readonly HashSet<Type> _repositories;
        private readonly Dictionary<Type, object> _initializedRepositories;

        public UnitOfWork()
        {
            Context = new();
            _repositories = new HashSet<Type>();
            _initializedRepositories = new();
        }

        public void AddRepository<R, TEntity>()
            where R : IRepository<T, TEntity> where TEntity : class
                => _repositories.Add(typeof(R));

        public void AddRepository(Type repositoryType)
            => _repositories.Add(repositoryType);

        public R GetRepository<R>()
        {
            if (_initializedRepositories.ContainsKey(typeof(R)))
            {
                return (R)_initializedRepositories[typeof(R)];
            }
            else
            {
                var type = _repositories.FirstOrDefault(x => x == typeof(R));
                if (type == null)
                    throw new InvalidOperationException($"{type} not added!");

                var obj = Activator.CreateInstance(type, new object[] { Context, this });
                if (obj == null)
                    throw new InvalidOperationException($"Cant CreateInstance of {type}");

                _initializedRepositories.Add(type, obj);
                return (R)obj;
            }
        }

        public IRepository<T, TEntity> GetBaseRepository<TEntity>()
            where TEntity : class
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