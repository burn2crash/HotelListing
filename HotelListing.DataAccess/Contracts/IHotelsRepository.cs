using HotelListing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelListing.DataAccess.Contracts
{
    public interface IHotelsRepository : IGenericRepository<Hotel>
    {
        void Update(Hotel hotel);
        Hotel GetById(int id);
        TResult GetById<TResult>(int id);
    }
}
