using System;
using System.Threading.Tasks;
using CP.Entities.Interfaces;
using CP.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CP.Repositories {
    public class Repository<TContext> : ReadOnlyRepository<TContext>, IRepository<TContext> where TContext : DbContext {
        public Repository (TContext context) : base (context) { }

        public virtual TEntity Create<TEntity> (TEntity entity, string createdBy = null) where TEntity : class, IEntity {
            entity.CreatedDate = DateTime.UtcNow;
            entity.CreatedBy = createdBy;
            _context.Set<TEntity> ().Add (entity);
            return entity;
        }

        public virtual TEntity Update<TEntity> (TEntity entity, string modifiedBy = null) where TEntity : class, IEntity {
            entity.ModifiedDate = DateTime.UtcNow;
            entity.ModifiedBy = modifiedBy;
            _context.Set<TEntity> ().Attach (entity);
            _context.Entry (entity).State = EntityState.Modified;
            return entity;
        }

        public virtual void Delete<TEntity> (object id, string deletedBy = null) where TEntity : class, IEntity {
            TEntity entity = _context.Set<TEntity> ().Find (id);
            Delete (entity, deletedBy);
        }

        public virtual void Delete<TEntity> (TEntity entity, string deletedBy = null) where TEntity : class, IEntity {
            var dbSet = _context.Set<TEntity> ();
            entity.Deleted = DateTime.UtcNow;
            entity.ModifiedBy = deletedBy;
            if (_context.Entry (entity).State == EntityState.Modified) {
                dbSet.Attach (entity);
            }
            _context.Entry (entity).State = EntityState.Modified;
        }

        public virtual Task SaveAsync () {
            return _context.SaveChangesAsync ();
        }
    }
}
