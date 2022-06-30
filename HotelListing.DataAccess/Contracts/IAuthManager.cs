using HotelListing.Models.DTOs.User;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelListing.DataAccess.Contracts
{
    public interface IAuthManager
    {
        Task<IEnumerable<IdentityError>> RegisterAsync(ApiUserDto userDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    }
}
