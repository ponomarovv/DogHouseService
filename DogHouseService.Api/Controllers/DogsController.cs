using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using DogHouseService.BLL.Interfaces;
using DogHouseService.BLL.Models;
using DogHouseService.Api.Models;

namespace DogHouseService.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class DogsController : ControllerBase
{
    private readonly IDogService _dogService;
    private readonly IMapper _mapper;

    public DogsController(IDogService dogService, IMapper mapper)
    {
        _dogService = dogService;
        _mapper = mapper;
    }

    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok("Dogshouseservice.Version1.0.1");
    }

    [HttpGet]
    public async Task<IActionResult> GetDogs([FromQuery] string attribute = null, [FromQuery] string order = null, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var dogs = await _dogService.GetDogsAsync(attribute, order, pageNumber, pageSize);
        var dogDtos = _mapper.Map<IEnumerable<DogDto>>(dogs);
        return Ok(dogDtos);
    }

    [HttpPost]
    public async Task<IActionResult> CreateDog([FromBody] DogDto dogDto)
    {
        try
        {
            var dogModel = _mapper.Map<DogModel>(dogDto);
            var createdDog = await _dogService.CreateDogAsync(dogModel);
            var createdDogDto = _mapper.Map<DogDto>(createdDog);
            return CreatedAtAction(nameof(GetDogs), new { id = createdDogDto.Id }, createdDogDto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}