using AutoMapper;
using HotelListing.DataAccess.Contracts;
using HotelListing.Models;
using HotelListing.Models.DTOs.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace HotelListing.DataAccess.Repository
{
    public class AuthManager : IAuthManager
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApiUser> _userManager;
        private readonly IConfiguration _config;
        private ApiUser _user;

        private string _loginProvider;
        private const string _refreshToken = "RefreshToken";

        public AuthManager(IMapper mapper, UserManager<ApiUser> userManager, IConfiguration config)
        {
            _mapper = mapper;
            _userManager = userManager;
            _config = config;

            _loginProvider = _config["JwtSettings:TokenProvider"];
        }

        public async Task<string> CreateRefreshToken()
        {
            await _userManager.RemoveAuthenticationTokenAsync(_user, _loginProvider, _refreshToken);
            var newRefrshToken = await _userManager.GenerateUserTokenAsync(_user, _loginProvider, _refreshToken);
            var result = await _userManager.SetAuthenticationTokenAsync(_user, _loginProvider, _refreshToken, newRefrshToken);

            return newRefrshToken;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            _user = await _userManager.FindByEmailAsync(loginDto.Email);
            var isValidUser = await _userManager.CheckPasswordAsync(_user, loginDto.Password);

            if (_user == null || !isValidUser)
                return null;

            var token = await generateTokenAsync();
            var response = new AuthResponseDto {
                Token = token,
                UserId = _user.Id,
                RefreshToken = await CreateRefreshToken()
            };

            return response;
        }

        public async Task<IEnumerable<IdentityError>> RegisterAsync(ApiUserDto userDto)
        {
            _user = _mapper.Map<ApiUser>(userDto);
            _user.UserName = _user.Email;

            var result = await _userManager.CreateAsync(_user, userDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(_user, "User");
            }

            return result.Errors;
        }

        public async Task<AuthResponseDto> VerifyRefrshToken(AuthResponseDto request)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(request.Token);
            var username = tokenContent.Claims.ToList().FirstOrDefault(q => q.Type ==
                JwtRegisteredClaimNames.Email)?.Value;
            _user = await _userManager.FindByNameAsync(username);

            if (_user == null || _user.Id != request.UserId)
                return null;

            var isValidRefreshToken = await _userManager.VerifyUserTokenAsync(_user, _loginProvider,
                _refreshToken, request.RefreshToken);

            if (isValidRefreshToken)
            {
                var token = await generateTokenAsync();
                return new AuthResponseDto {
                    Token = token,
                    UserId = _user.Id,
                    RefreshToken = await CreateRefreshToken()
                };
            }

            await _userManager.UpdateSecurityStampAsync(_user);

            return null;
        }

        private async Task<string> generateTokenAsync()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var roles = await _userManager.GetRolesAsync(_user);
            var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x));
            var userClaims = await _userManager.GetClaimsAsync(_user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, _user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, _user.Email),
                new Claim("uid", _user.Id)
            }
            .Union(roleClaims).Union(userClaims);

            var now = DateTime.UtcNow;
            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(Convert.ToInt32(_config["JwtSettings:DurationInMinutes"])),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
