﻿
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interface;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : Controller
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;

        public OwnerController(IOwnerRepository ownerRepository,ICountryRepository countryRepository, IMapper mapper)
        {
            _ownerRepository = ownerRepository;
            _countryRepository = countryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Owner))]
        public IActionResult GetOwners()
        {
            var owners = _mapper.Map<List<OwnerDto>>(_ownerRepository.GetOwners());
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(owners);
        }

        [HttpGet("{ownerId}")]
        [ProducesResponseType(200, Type = typeof(Owner))]
        [ProducesResponseType(400)]
        public IActionResult GetOwner(int ownerId) 
        {
            if (!_ownerRepository.OwnerExists(ownerId))
                return NotFound();
            var owner  = _mapper.Map<Owner>(_ownerRepository.GetOwner(ownerId));
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(owner);   
        }

        [HttpGet("/owner/{pokeId}")]
        [ProducesResponseType(200,Type = typeof(Owner))]
        public IActionResult GetOwnerOfPolemon(int pokeId)
        {
            var owner  = _ownerRepository.GetOwnerOfPokemon(pokeId);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(owner);    
        }

        [HttpGet("/{ownerId}/pokemon")]
        [ProducesResponseType(200, Type = typeof(Owner))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonByOwner(int ownerId)
        {
            if (!_ownerRepository.OwnerExists(ownerId)) 
                return NotFound();  
            var pokemons = _ownerRepository.GetPokemonByOwner(ownerId);
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(pokemons);    
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateOwner([FromBody] OwnerDto ownerDto,[FromQuery] int countryId)
        {
            if(ownerDto == null)
            {
                return BadRequest(ModelState);  
            }
            var owner = _ownerRepository.GetOwners()
                .Where(o => o.LastName.Trim().ToUpper() == ownerDto.LastName.TrimEnd().ToUpper())
                .FirstOrDefault();
            if(owner != null)
            {
                ModelState.AddModelError("", "Owner already exists");
                return StatusCode(422, ModelState);
                    
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var ownerMap = _mapper.Map<Owner>(ownerDto);

            ownerMap.Country = _countryRepository.GetCountry(countryId);

            if (!_ownerRepository.CreateOwner(ownerMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving !!!");
                return StatusCode(500,ModelState);  
            }
            return Ok("Successfully created");

        }

        [HttpPut("{ownerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCategory(int ownerId, [FromBody] OwnerDto ownerDto)
        {
            if (ownerDto == null)
            {
                return BadRequest(ModelState);
            }
            if (ownerId != ownerDto.Id)
                return BadRequest(ModelState);
            if (!_ownerRepository.OwnerExists(ownerId))
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
                return BadRequest();
            var ownerMap = _mapper.Map<Owner>(ownerDto);
            if (!_ownerRepository.UpdateOwner(ownerMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
        [HttpDelete("{ownerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteOwner(int ownerId)
        {
            if (!_ownerRepository.OwnerExists(ownerId))
            {
                return NotFound();
            }
            var ownerToDelete = _ownerRepository.GetOwner(ownerId);
           
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_ownerRepository.DeleteOwner(ownerToDelete))
            {
                ModelState.AddModelError("", "Something went wrong while deleted owner");
            }
            return NoContent();
        }

    }
}
