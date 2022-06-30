using HotelListing.Models.DTOs.Country;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelListing.Models.DTOs.Hotel
{
    public class HotelDto : GetHotelDto
    {
        public int CountryId { get; set; }
        //public CountryDto Country { get; set; }
    }
}
