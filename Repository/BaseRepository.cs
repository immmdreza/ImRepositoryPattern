using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ImRepositoryPattern.Repository
{
    public class BaseRepository<TContext, TEntity> :
        IRepository<TContext, TEntity> where TContext : DbContext where TEntity : class
    {
        public BaseRepository(TContext context)
        {
            Context = context;
        }

        public TContext Context { get; }

        public DbSet<TEntity> EntitySet => Context.Set<TEntity>();


        public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
        {
            return await Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual void Insert(TEntity entity)
        {
            EntitySet.Add(entity);
        }

        public virtual async Task<IEnumerable<TEntity>> FindAsync(
                    Expression<Func<TEntity, bool>>? filter = null,
                    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                    string includeProperties = "",
                    CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = EntitySet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else
            {
                return await query.ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            }
        }

        public virtual async Task<TEntity?> FindOneAsync(
                    Expression<Func<TEntity, bool>>? filter = null,
                    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                    string includeProperties = "",
                    CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = EntitySet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return await orderBy(query).SingleOrDefaultAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else
            {
                return await query.SingleOrDefaultAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            }
        }

        public virtual async Task<bool?> ExistsAsync(
                    Expression<Func<TEntity, bool>>? filter = null,
                    CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = EntitySet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.AnyAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task<TEntity?> GetByIDAsync(object id, CancellationToken cancellationToken = default)
        {
            return await EntitySet.FindAsync(new object?[] { id }, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public virtual void Delete(object id)
        {
            TEntity? entityToDelete = EntitySet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity? entityToDelete)
        {
            if (entityToDelete == null)
                return;

            if (Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                EntitySet.Attach(entityToDelete);
            }

            EntitySet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            EntitySet.Attach(entityToUpdate);
            Context.Entry(entityToUpdate).State = EntityState.Modified;
        }
    }
}
