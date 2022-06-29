using HotelListing.DataAccess.Contracts;
using HotelListing.DataAccess.Data;
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
        internal DbSet<T> dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
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

        IEnumerable<T> IGenericRepository<T>.GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, bool tracked = true)
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

            return query.ToList();
        }

        T IGenericRepository<T>.GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            query = IncludeProperties(query, includeProperties);
            query = query.Where(filter);

            return query.FirstOrDefault();
        }

        void IGenericRepository<T>.Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        void IGenericRepository<T>.RemoveRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }

        public bool Exists(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = dbSet;

            query = query.Where(filter);
            return query.Any();
        }
    }
}
