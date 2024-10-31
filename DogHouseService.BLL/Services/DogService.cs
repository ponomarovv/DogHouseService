using Microsoft.EntityFrameworkCore;
using AutoMapper;
using DogHouseService.BLL.Interfaces;
using DogHouseService.BLL.Models;
using DogHouseService.DAL.Data;
using DogHouseService.DAL.Models;
using DogHouseService.BLL.Extensions;

namespace DogHouseService.BLL.Services;

public class DogService : IDogService
{
    private readonly DogHouseContext _context;
    private readonly IMapper _mapper;

    public DogService(DogHouseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Asynchronously retrieves a paginated list of dogs from the database, with optional sorting by a specified attribute.
    /// </summary>
    /// <param name="attribute">The attribute to sort by, such as "Name", "Color", "TailLength" or "Weight".</param>
    /// <param name="order">The order of sorting, either "asc" for ascending or "desc" for descending.</param>
    /// <param name="pageNumber">The page number for pagination.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A task representing the asynchronous operation, containing a paginated and sorted list of <see cref="DogModel"/> instances.</returns>

    public async Task<IEnumerable<DogModel>> GetDogsAsync(string attribute = null, string order = null, int pageNumber = 1, int pageSize = 10)
    {
        var query = _context.Dogs.AsQueryable();

        if (!string.IsNullOrEmpty(attribute) && !string.IsNullOrEmpty(order))
        {
            query = query.OrderBy(attribute, order.ToLower() == "asc");
        }

        var dogs = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return _mapper.Map<IEnumerable<DogModel>>(dogs);
    }

    /// <summary>
    /// Asynchronously creates a new dog entry in the database based on the provided <see cref="DogModel"/>.
    /// </summary>
    /// <param name="dogModel">The dog model containing information about the dog to be created.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="DogModel"/>.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a dog with the same name already exists or if the tail length is a negative value.
    /// </exception>
    public async Task<DogModel> CreateDogAsync(DogModel dogModel)
    {
        if (await _context.Dogs.AnyAsync(d => d.Name == dogModel.Name))
        {
            throw new InvalidOperationException("A dog with the same name already exists.");
        }

        if (dogModel.TailLength < 0)
        {
            throw new InvalidOperationException("Tail length cannot be a negative number.");
        }

        var dog = _mapper.Map<Dog>(dogModel);
        _context.Dogs.Add(dog);
        await _context.SaveChangesAsync();

        return _mapper.Map<DogModel>(dog);
    }
}
