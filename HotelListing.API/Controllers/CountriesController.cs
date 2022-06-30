using AutoMapper;
using HotelListing.DataAccess.Contracts;
using HotelListing.Models;
using HotelListing.Models.DTOs.Country;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _context;

        public CountriesController(IMapper mapper, IUnitOfWork context)
        {
            _mapper = mapper;
            _context = context;
        }

        // GET: api/<CountriesController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var countryDtoList = _mapper.Map<List<CountryDto>>(_context.Countries.GetAll());
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
                return NotFound();

            var countryDto = _mapper.Map<CountryDto>(country);

            return Ok(countryDto);
        }

        // POST api/<CountriesController>
        [HttpPost]
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
        public IActionResult Put(int id, [FromBody] UpdateCountryDto countryDto)
        {
            if (countryDto == null)
                return BadRequest("Country data must be provided");

            if (id != countryDto.Id)
                return BadRequest("Id does not match");

            var country = _context.Countries.GetById(id);

            if (country == null)
                return NotFound();

            _mapper.Map(countryDto, country);

            _context.Countries.Update(country);

            try
            {
                _context.Save();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!_context.Countries.Exists(h => h.Id == id))
                {
                    return NotFound();
                }
                else throw;
            }

            return NoContent();
        }

        // DELETE api/<CountriesController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
                return BadRequest();

            var country = _context.Countries.GetById(id);

            if (country == null)
                return NotFound();

            _context.Countries.Remove(country);
            _context.Save();

            return NoContent();
        }
    }
}
