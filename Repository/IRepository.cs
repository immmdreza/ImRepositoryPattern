using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ImRepositoryPattern.Repository
{
    public interface IRepository<TContext, TEntity>
        where TContext : DbContext, new() where TEntity : class
    {
        TContext Context { get; }

        DbSet<TEntity> EntitySet { get; }

        UnitOfWork<TContext> UnitOfWork { get; }


        void Insert(TEntity entity);

        void Delete(object id);

        void Delete(TEntity? entityToDelete);

        void Update(TEntity entityToUpdate);

        public Task<int> SaveAsync();

        Task<IEnumerable<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "");

        Task<TEntity?> FindOneAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "");

        Task<bool?> ExistsAsync(
            Expression<Func<TEntity, bool>>? filter = null);

        Task<TEntity?> GetByIDAsync(object id);
    }
}
