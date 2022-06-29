using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelListing.Models.DTOs.Country
{
    public class UpdateCountryDto : CountryBase
    {
        public int Id { get; set; }
    }
}
