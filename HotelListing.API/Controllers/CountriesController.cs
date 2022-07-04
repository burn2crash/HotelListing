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
    [ApiVersion("1.0", Deprecated = true)]
    public class CountriesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _context;
        private readonly ILogger<CountriesController> _logger;

        public CountriesController(IMapper mapper, IUnitOfWork context, ILogger<CountriesController> logger)
        {
            _mapper = mapper;
            _context = context;
            _logger = logger;
        }

        // GET: api/<CountriesController>
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetCountriesAsync()
        {
            //var countryDtoList = _mapper.Map<List<GetCountryDto>>(await _context.Countries.GetAllAsync());
            var countryDtoList = await _context.Countries.GetAllAsync<GetCountryDto>();
            return Ok(countryDtoList);
        }

        // GET: api/<CountriesController>/?StartIndex=0&PageSize=25&PageNumber=1
        [HttpGet]
        public async Task<IActionResult> GetPagedCountriesAsync([FromQuery] 
            QueryParameters queryParameters)
        {
            var pagedCountriesResult = await _context.Countries.GetAllAsync<GetCountryDto>(queryParameters);
            return Ok(pagedCountriesResult);
        }

        // GET api/<CountriesController>/5
        [HttpGet("{id}", Name = "GetCountry")]
        public IActionResult GetCountry(int id)
        {
            if (id == 0)
                return BadRequest();

            //var country = _context.Countries.GetById(id);
            var country = _context.Countries.GetById<CountryDto>(id);

            if (country == null)
            {
                //_logger.LogWarning($"No record found in {nameof(GetCountry)} with Id: {id}");
                //return NotFound();
                throw new NotFoundException(nameof(GetCountry), id);
            }

            //var countryDto = _mapper.Map<CountryDto>(country);

            //return Ok(countryDto);
            return Ok(country);
        }

        // POST api/<CountriesController>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] CreateCountryDto countryDto)
        {
            if (countryDto == null)
                return BadRequest();

            var country = _mapper.Map<Country>(countryDto);
            
            await _context.Countries.AddAsync(country);
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
