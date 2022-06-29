using HotelListing.Models.DTOs.Hotel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelListing.Models.DTOs.Country
{
    public class CountryDto : GetCountryDto
    {
        public List<HotelDto> Hotels { get; set; }
    }
}
