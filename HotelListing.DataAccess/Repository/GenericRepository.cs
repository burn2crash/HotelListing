using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.DataAccess.Contracts;
using HotelListing.DataAccess.Data;
using HotelListing.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
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

        public async Task AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        //private IQueryable<T> IncludeProperties(IQueryable<T> query, string? includeProperties)
        //{
        //    if (includeProperties != null)
        //    {
        //        foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        //        {
        //            query = query.Include(includeProp);
        //        }
        //    }
        //    return query;
        //}

        private IQueryable<T> getAllQuery(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, bool tracked = true)
        {
            IQueryable<T> query;

            if (tracked)
            {
                query = dbSet;
            }
            else
                query = dbSet.AsNoTracking();

            //query = IncludeProperties(query, includeProperties);
            if (filter != null)
                query = query.Where(filter);

            if (include != null)
            {
                query = include(query);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }


            return query;
        }
        private IQueryable<TResult> getAllQuery<TResult>(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, bool tracked = true)
        {
            IQueryable<T> query = getAllQuery(filter, orderBy, include, tracked);

            return query.ProjectTo<TResult>(_mapper.ConfigurationProvider);
        }

        public async Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, bool tracked = true)
        {
            IQueryable<TResult> query = getAllQuery<TResult>(filter, orderBy, include, tracked);

            return await query.ToListAsync();
        }

        public IEnumerable<TResult> GetAll<TResult>(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, bool tracked = true)
        {
            IQueryable<TResult> query = getAllQuery<TResult>(filter, orderBy, include, tracked);

            return query.ToList();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, bool tracked = true)
        {
            IQueryable<T> query = getAllQuery(filter, orderBy, include, tracked);

            return await query.ToListAsync();
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, bool tracked = true)
        {
            IQueryable<T> query = getAllQuery(filter, orderBy, include, tracked);

            return query.ToList();
        }

        public async Task<PagedResult<TResult>> GetAllAsync<TResult>(QueryParameters queryParameters, Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, bool tracked = true)
        {
            IQueryable<T> query = getAllQuery(filter, orderBy, include, tracked);
            var pagedResult = await getPagedResultAsync<TResult>(query, queryParameters);
            
            return pagedResult;
        }

        public PagedResult<TResult> GetAll<TResult>(QueryParameters queryParameters, Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, bool tracked = true)
        {
            IQueryable<T> query = getAllQuery(filter, orderBy, include, tracked);
            var totalSize = query.Count();
            var items = query
                .Skip(queryParameters.StartIndex)
                .Take(queryParameters.PageSize)
                .ProjectTo<TResult>(_mapper.ConfigurationProvider)
                .ToList();
            return new PagedResult<TResult>
            {
                Items = items,
                PageNumber = queryParameters.PageNumber,
                RecordNumber = queryParameters.PageSize,
                TotalCount = totalSize
            };
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

        public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            var query = getFirstOrDefault(filter, include);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<TResult> GetFirstOrDefaultAsync<TResult>(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            var query = getFirstOrDefault(filter, include);

            return await query.ProjectTo<TResult>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            var query = getFirstOrDefault(filter, include);

            return query.FirstOrDefault();
        }

        public TResult GetFirstOrDefault<TResult>(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            var query = getFirstOrDefault(filter, include);
            
            return query.ProjectTo<TResult>(_mapper.ConfigurationProvider).FirstOrDefault();
        }

        private IQueryable<T> getFirstOrDefault(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            IQueryable<T> query = dbSet;

            //query = IncludeProperties(query, includeProperties);
            if (include != null)
            {
                query = include(query);
            }
            query = query.Where(filter);

            return query;
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

        public bool Exists(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = dbSet;

            query = query.Where(filter);
            return query.Any();
        }
    }
}
