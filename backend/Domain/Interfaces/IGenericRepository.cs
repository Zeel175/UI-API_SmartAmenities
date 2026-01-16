using Domain.Entities;
using Domain.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(long id);
        Task AddAsync(T entity, string loginUser, string action);
        Task UpdateAsync(T entity, string loginUser, string action);
        Task DeleteAsync(long id, string loginUser, string action);

        void DeleteEntity(T entityToDelete);

        IQueryable<T> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            int? pageIndex = null,
            int? pageSize = null,
            string includeProperties = "");

        Task<PaginatedList<T>> GetPagedDataAsync(int pageNumber, int pageSize);
    }
}