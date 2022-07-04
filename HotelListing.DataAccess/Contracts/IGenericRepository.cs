using HotelListing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HotelListing.DataAccess.Contracts
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter, string? includeProperties = null);
        Task<TResult> GetFirstOrDefaultAsync<TResult>(Expression<Func<T, bool>> filter, string? includeProperties = null);
        T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null);
        TResult GetFirstOrDefault<TResult>(Expression<Func<T, bool>> filter, string? includeProperties = null);
        //T GetById(int id);
        Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, bool tracked = true);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, bool tracked = true);
        IEnumerable<TResult> GetAll<TResult>(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, bool tracked = true);
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, bool tracked = true);
        Task<PagedResult<TResult>> GetAllAsync<TResult>(QueryParameters queryParameters, Expression<Func<T, bool>>? filter = null, string? includeProperties = null, bool tracked = true);
        PagedResult<TResult> GetAll<TResult>(QueryParameters queryParameters, Expression<Func<T, bool>>? filter = null, string? includeProperties = null, bool tracked = true);
        Task AddAsync(T entity);
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> filter);
        bool Exists(Expression<Func<T, bool>> filter);
    }
}
