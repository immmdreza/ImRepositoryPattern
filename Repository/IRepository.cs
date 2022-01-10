using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ImRepositoryPattern.Repository
{
    public interface IRepository<TContext, TEntity>
        where TContext : DbContext where TEntity : class
    {
        /// <summary>
        /// It's the operating <see cref="DbContext"/>.
        /// </summary>
        TContext Context { get; }

        /// <summary>
        /// <see cref="DbSet{TEntity}"/> of this repository ( your db table ).
        /// </summary>
        DbSet<TEntity> EntitySet { get; }

        /// <summary>
        /// It's the operating <see cref="IUnitOfWork{T}"/>.
        /// </summary>
        IUnitOfWork<TContext> UnitOfWork { get; }

        /// <summary>
        /// Inserts a <see cref="TEntity"/>.
        /// </summary>
        /// <remarks>You need to call <see cref="SaveAsync"/>.</remarks>
        /// <param name="entity"></param>
        void Insert(TEntity entity);

        /// <summary>
        /// Deletes an object by its id.
        /// </summary>
        /// <remarks>You need to call <see cref="SaveAsync"/>.</remarks>
        /// <param name="id"></param>
        void Delete(object id);

        /// <summary>
        /// Deletes a <see cref="TEntity"/>
        /// </summary>
        /// <remarks>You need to call <see cref="SaveAsync"/>.</remarks>
        /// <param name="entityToDelete"></param>
        void Delete(TEntity? entityToDelete);

        /// <summary>
        /// Updates a <see cref="TEntity"/>
        /// </summary>
        /// <remarks>You need to call <see cref="SaveAsync"/>.</remarks>
        /// <param name="entityToUpdate"></param>
        void Update(TEntity entityToUpdate);

        /// <summary>
        /// Saves changes.
        /// </summary>
        public Task<int> SaveAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "", 
            CancellationToken cancellationToken = default);

        Task<TEntity?> FindOneAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "",
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if an entity exists.
        /// </summary>
        Task<bool?> ExistsAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            CancellationToken cancellationToken = default);

        Task<TEntity?> GetByIDAsync(object id, CancellationToken cancellationToken = default);
    }
}
