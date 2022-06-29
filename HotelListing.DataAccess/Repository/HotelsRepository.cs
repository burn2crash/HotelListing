using HotelListing.DataAccess.Contracts;
using HotelListing.DataAccess.Data;
using HotelListing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelListing.DataAccess.Repository
{
    public class HotelsRepository : GenericRepository<Hotel>, IHotelsRepository
    {
        private readonly ApplicationDbContext _context;

        public HotelsRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Hotel hotel)
        {
            _context.Hotels.Update(hotel);
        }
    }
}
