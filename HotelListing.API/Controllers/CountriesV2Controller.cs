using AutoMapper;
using HotelListing.DataAccess.Contracts;
using HotelListing.Models;
using HotelListing.Models.DTOs.Country;
using HotelListing.Utility;
using HotelListing.Utility.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HotelListing.API.Controllers
{
    [Route("api/v{version:apiVersion}/countries")]
    [ApiController]
    [ApiVersion("2.0")]
    public class CountriesV2Controller : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _context;
        private readonly ILogger<CountriesV2Controller> _logger;

        public CountriesV2Controller(IMapper mapper, IUnitOfWork context, ILogger<CountriesV2Controller> logger)
        {
            _mapper = mapper;
            _context = context;
            _logger = logger;
        }

        // GET: api/<CountriesController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var countryDtoList = _mapper.Map<List<CountryDto>>(await _context.Countries.GetAllAsync());
            return Ok(countryDtoList);
        }

        // GET api/<CountriesController>/5
        [HttpGet("{id}", Name = "GetCountry")]
        public IActionResult GetCountry(int id)
        {
            if (id == 0)
                return BadRequest();

            var country = _context.Countries.GetById(id);

            if (country == null)
            {
                //_logger.LogWarning($"No record found in {nameof(GetCountry)} with Id: {id}");
                //return NotFound();
                throw new NotFoundException(nameof(GetCountry), id);
            }

            var countryDto = _mapper.Map<CountryDto>(country);

            return Ok(countryDto);
        }

        // POST api/<CountriesController>
        [HttpPost]
        [Authorize]
        public IActionResult Post([FromBody] CreateCountryDto countryDto)
        {
            if (countryDto == null)
                return BadRequest();

            var country = _mapper.Map<Country>(countryDto);
            
            _context.Countries.Add(country);
            _context.Save();

            return CreatedAtAction("GetCountry", new { id = country.Id }, country);
        }

        // PUT api/<CountriesController>/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateCountryDto countryDto)
        {
            if (countryDto == null)
                return BadRequest("Country data must be provided");

            if (id != countryDto.Id)
                return BadRequest("Id does not match");

            var country = _context.Countries.GetById(id);

            if (country == null)
            {
                //return NotFound();
                throw new NotFoundException(nameof(Put), id);
            }

            _mapper.Map(countryDto, country);

            _context.Countries.Update(country);

            try
            {
                _context.Save();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!await _context.Countries.ExistsAsync(h => h.Id == id))
                {
                    return NotFound();
                }
                else throw;
            }

            return NoContent();
        }

        // DELETE api/<CountriesController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = SD.Roles_Admin)]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
                return BadRequest();

            var country = _context.Countries.GetById(id);

            if (country == null)
            {
                //return NotFound();
                throw new NotFoundException(nameof(Delete), id);
            }

            _context.Countries.Remove(country);
            _context.Save();

            return NoContent();
        }
    }
}
