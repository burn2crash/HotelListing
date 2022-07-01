using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.DataAccess.Contracts;
using HotelListing.DataAccess.Data;
using HotelListing.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HotelListing.DataAccess.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        internal DbSet<T> dbSet;

        public GenericRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            dbSet = _context.Set<T>();
        }

        void IGenericRepository<T>.Add(T entity)
        {
            dbSet.Add(entity);
        }

        private IQueryable<T> IncludeProperties(IQueryable<T> query, string? includeProperties)
        {
            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query;
        }

        private IQueryable<T> getAllQuery(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, bool tracked = true)
        {
            IQueryable<T> query;

            if (tracked)
            {
                query = dbSet;
            }
            else
                query = dbSet.AsNoTracking();

            query = IncludeProperties(query, includeProperties);
            if (filter != null)
                query = query.Where(filter);

            return query;
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, bool tracked = true)
        {
            IQueryable<T> query = getAllQuery(filter, includeProperties, tracked);

            return await query.ToListAsync();
        }

        public async Task<PagedResult<TResult>> GetAllAsync<TResult>(QueryParameters queryParameters, Expression<Func<T, bool>>? filter = null, string? includeProperties = null, bool tracked = true)
        {
            IQueryable<T> query = getAllQuery(filter, includeProperties, tracked);
            var pagedResult = await getPagedResultAsync<TResult>(query, queryParameters);
            
            return pagedResult;
        }

        private async Task<PagedResult<TResult>> getPagedResultAsync<TResult>(IQueryable<T> query, QueryParameters queryParameters)
        {
            var totalSize = await query.CountAsync();
            var items = await query
                .Skip(queryParameters.StartIndex)
                .Take(queryParameters.PageSize)
                .ProjectTo<TResult>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return new PagedResult<TResult>
            {
                Items = items,
                PageNumber = queryParameters.PageNumber,
                RecordNumber = queryParameters.PageSize,
                TotalCount = totalSize
            };
        }

        public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            query = IncludeProperties(query, includeProperties);
            query = query.Where(filter);

            return await query.FirstOrDefaultAsync();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = dbSet;

            query = query.Where(filter);
            return await query.AnyAsync();
        }
    }
}
