using Domain.Entities;
using Domain.Interfaces;
using Domain.ViewModels;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using static System.Collections.Specialized.BitVector32;


namespace Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        public readonly AppDbContext _context;
        public readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T> GetByIdAsync(long id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
        
        public async Task AddAsync(T entity, string loginUser, string action)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();

            // Audit Logging
            var auditLogs = AuditHelper.GenerateAuditLogs(null, entity, typeof(T).Name, loginUser, action);
            foreach (var log in auditLogs)
            {
                await _context.Set<AuditLog>().AddAsync(log);
            }
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity, string LoginUser, string Action)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            //try
            //{
            //    // Detach any tracked entity with the same key
            //    var trackedEntity = _context.ChangeTracker.Entries<T>().FirstOrDefault(e => e.Entity == entity);
            //    if (trackedEntity != null)
            //    {
            //        _context.Entry(trackedEntity.Entity).State = EntityState.Detached;
            //    }

            //    // Get the primary key value
            //    var key = GetEntityKey(entity);
            //    //if (key == null || key.All(k => k == null))
            //    //    throw new Exception("Entity key is not set or invalid.");

            //    // Fetch the existing tracked entity
            //    var existingEntity = await _dbSet.FindAsync(key);
            //    if (existingEntity == null)
            //        throw new Exception("Entity not found.");

            //    // Debugging tip: Check the values of `existingEntity` here
            //    // Log or inspect the existing entity's state
            //    Console.WriteLine("Existing Entity:", existingEntity);

            //    // Perform audit logging before making changes
            //    var auditLogs = AuditHelper.GenerateAuditLogs(existingEntity, entity, typeof(T).Name, LoginUser, Action);
            //    if (auditLogs != null && auditLogs.Any())
            //    {
            //        await _context.Set<AuditLog>().AddRangeAsync(auditLogs);
            //    }

            //    // Update the tracked entity's values
            //    _context.Entry(existingEntity).CurrentValues.SetValues(entity);

            //    // Save changes
            //    await _context.SaveChangesAsync();

            //    // Debugging tip: Check the values of `existingEntity` after SaveChanges
            //    Console.WriteLine("Updated Entity:", existingEntity);
            //}
            //catch (Exception ex)
            //{
            //    // Handle or log the exception
            //    throw;
            //}


        }

        public async Task DeleteAsync(long id, string LoginUser, string Action)
        {
            var entity = await _dbSet.FindAsync(id);

            // Audit Logging
            var auditLogs = AuditHelper.GenerateAuditLogs(entity, null, typeof(T).Name, LoginUser, Action);
            foreach (var log in auditLogs)
            {
                await _context.Set<AuditLog>().AddAsync(log);
            }


            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public IQueryable<T> Get(
       Expression<Func<T, bool>> filter = null,
       Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
       int? pageIndex = null,
       int? pageSize = null,
       string includeProperties = "")
        {
            IQueryable<T> query = _dbSet;

            // Apply filter
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Include properties
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            // Apply ordering
            if (orderBy != null)
            {
                query = orderBy(query);
            }

            // Apply pagination
            if (pageIndex != null && pageSize != null)
            {
                query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }

            return query;
        }

        public void DeleteEntity(T entityToDelete)
        {
            if (entityToDelete == null)
            {
                throw new ArgumentNullException(nameof(entityToDelete));
            }

            _dbSet.Remove(entityToDelete);
        }
 

        public async Task<PaginatedList<T>> GetPagedDataAsync(int pageNumber, int pageSize)
        {
            var source = _context.Set<T>().AsQueryable();
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize)
                                     .Take(pageSize)
                                     .ToListAsync();

            return new PaginatedList<T>(items, count, pageNumber, pageSize);
        }

        // Helper to get the key of an entity
        private object GetEntityKey(T entity)
        {
            var keyName = _context.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties
                .Select(x => x.Name).Single();
            return typeof(T).GetProperty(keyName)?.GetValue(entity, null);
        }
    }
}