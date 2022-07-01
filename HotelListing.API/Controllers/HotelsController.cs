using AutoMapper;
using HotelListing.DataAccess.Contracts;
using HotelListing.Models;
using HotelListing.Models.DTOs.Hotel;
using HotelListing.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly IUnitOfWork _context;
        private readonly IMapper _mapper;

        public HotelsController(IMapper mapper, IUnitOfWork context)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var hotels = await _context.Hotels.GetAllAsync();
            var hotelDtoList = _mapper.Map<List<HotelDto>>(hotels);

            return Ok(hotelDtoList);
        }

        [HttpGet("{id}", Name = "GetHotel")]
        public async Task<IActionResult> Get(int id)
        {
            if (id <= 0)
                return BadRequest();

            var hotel = _context.Hotels.GetById(id);

            if (hotel == null)
                return NotFound();

            var hotelDto = _mapper.Map<HotelDto>(hotel);

            return Ok(hotelDto);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post(CreateHotelDto hotelDto)
        {
            if (hotelDto == null)
                return BadRequest();

            var hotel = _mapper.Map<Hotel>(hotelDto);

            _context.Hotels.Add(hotel);
            _context.Save();

            return CreatedAtAction("GetHotel", new { id = hotel.Id }, hotel);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, HotelDto hotelDto)
        {
            if (hotelDto == null)
                return BadRequest("Hotel data must be provided");

            if (id != hotelDto.Id)
                return BadRequest("Id does not match");

            var hotel = _context.Hotels.GetById(id);

            if (hotel == null)
                return NotFound();

            _mapper.Map(hotelDto, hotel);

            _context.Hotels.Update(hotel);

            try
            {
                _context.Save();
            }
            catch(DbUpdateConcurrencyException ex)
            {
                if (!await _context.Hotels.ExistsAsync(h => h.Id == id))
                {
                    return NotFound();
                }
                else throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = SD.Roles_Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest();

            var hotel = _context.Hotels.GetById(id);

            if (hotel == null)
                return NotFound();

            _context.Hotels.Remove(hotel);
            _context.Save();

            return NoContent();
        }
    }
}
