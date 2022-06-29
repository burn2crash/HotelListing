using HotelListing.DataAccess.Contracts;
using HotelListing.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelListing.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public ICountriesRepository Countries { get; private set; }
        public IHotelsRepository Hotels { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            Countries = new CountriesRepository(context);
            Hotels = new HotelsRepository(context);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
