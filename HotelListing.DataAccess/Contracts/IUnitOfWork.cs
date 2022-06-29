using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelListing.DataAccess.Contracts
{
    public interface IUnitOfWork
    {
        ICountriesRepository Countries { get; }
        IHotelsRepository Hotels { get; }

        public void Save();
    }
}
