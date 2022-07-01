using AutoMapper;
using HotelListing.DataAccess.Contracts;
using HotelListing.DataAccess.Data;
using HotelListing.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
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
        //public IAuthManager Authentication { get; private set; }

        public UnitOfWork(ApplicationDbContext context, IMapper mapper
            //, UserManager<ApiUser> userManager, 
            //IConfiguration config
            )
        {
            _context = context;

            Countries = new CountriesRepository(context, mapper);
            Hotels = new HotelsRepository(context, mapper);

            //Authentication = new AuthManager(mapper, userManager, config);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
