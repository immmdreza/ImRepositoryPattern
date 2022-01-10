using ImRepositoryPattern.Repository;
using Microsoft.EntityFrameworkCore;

namespace ImRepositoryPattern
{
    /// <summary>
    /// Represents an unit of work for specified <see cref="DbContext"/> as <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IUnitOfWork<T> : IDisposable where T : DbContext, new()
    {
        /// <summary>
        /// The <see cref="DbContext"/> instance of type <typeparamref name="T"/>.
        /// </summary>
        public T Context { get; }

        /// <summary>
        /// Adds an <see cref="IRepository{TContext, TEntity}"/> to use it later.
        /// </summary>
        /// <remarks>
        /// Repository must have a constructor that takes only two params: <see cref="DbContext"/> and <see cref="UnitOfWork"/> 
        /// </remarks>
        /// <param name="uniqueName">Unique tag for this repository</param>
        public void AddRepository<R, TEntity>()
            where R : IRepository<T, TEntity> where TEntity : class;

        /// <summary>
        /// Adds an <see cref="IRepository{TContext, TEntity}"/> to use it later.
        /// </summary>
        /// <remarks>
        /// Repository must have a constructor that takes only two params: <see cref="DbContext"/> and <see cref="IUnitOfWork{T}"/> 
        /// </remarks>
        public void AddRepository(Type repositoryType);

        /// <summary>
        /// Gets an <see cref="IRepository{TContext, TEntity}"/>, which added before using <see cref="AddRepository{R, TContext, TEntity}()"/>
        /// Or <see cref="AddRepository(Type)"/>.
        /// </summary>
        /// <remarks>This method is safe to call more than once for a single <typeparamref name="TEntity"/>.
        /// No new instance will be created!
        /// </remarks>
        /// <typeparam name="R">The <see cref="IRepository{TContext, TEntity}"/> exact type</typeparam>
        /// <returns></returns>
        public R GetRepository<R>();

        /// <summary>
        /// Get a basic repository for specified <typeparamref name="TEntity"/>
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public IRepository<T, TEntity> GetBaseRepository<TEntity>()
            where TEntity : class;

        /// <summary>
        /// Save changes for <typeparamref name="T"/>
        /// </summary>
        /// <returns></returns>
        public Task<int> SaveAsync();
    }
}
