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
    public class CountriesRepository : GenericRepository<Country>, ICountriesRepository
    {
        private readonly ApplicationDbContext _context;

        public CountriesRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public Country GetById(int id)
        {
            return _context.Contries.Include(c => c.Hotels).FirstOrDefault(c => c.Id == id);
        }

        public void Update(Country country)
        {
            _context.Contries.Update(country);
        }
    }
}
