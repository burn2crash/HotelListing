using AutoMapper;
using HotelListing.DataAccess.Contracts;
using HotelListing.DataAccess.Data;
using HotelListing.Models;
using Microsoft.EntityFrameworkCore;
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

        public HotelsRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
        }

        public Hotel GetById(int id)
        {
            //return _context.Hotels.Include(h => h.Country).FirstOrDefault(h => h.Id == id);
            return _context.Hotels.FirstOrDefault(h => h.Id == id);
        }

        public void Update(Hotel hotel)
        {
            _context.Hotels.Update(hotel);
        }
    }
}
