using HotelListing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelListing.DataAccess.Contracts
{
    public interface ICountriesRepository : IGenericRepository<Country>
    {
        void Update(Country country);
        Country GetById(int id);
        TResult GetById<TResult>(int id);
    }
}
