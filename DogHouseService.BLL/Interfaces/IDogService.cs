using DogHouseService.BLL.Models;

namespace DogHouseService.BLL.Interfaces;

public interface IDogService
{
    /// <summary>
    /// Asynchronously retrieves a paginated list of dogs from the database, with optional sorting by a specified attribute.
    /// </summary>
    /// <param name="attribute">The attribute to sort by, such as "Name", "Color", "TailLength" or "Weight".</param>
    /// <param name="order">The order of sorting, either "asc" for ascending or "desc" for descending.</param>
    /// <param name="pageNumber">The page number for pagination.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A task representing the asynchronous operation, containing a paginated and sorted list of <see cref="DogModel"/> instances.</returns>
    Task<IEnumerable<DogModel>> GetDogsAsync(string attribute = null, string order = null, int pageNumber = 1, int pageSize = 10);
    
    /// <summary>
    /// Asynchronously creates a new dog entry in the database based on the provided <see cref="DogModel"/>.
    /// </summary>
    /// <param name="dogModel">The dog model containing information about the dog to be created.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="DogModel"/>.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a dog with the same name already exists or if the tail length is a negative value.
    /// </exception>
    Task<DogModel> CreateDogAsync(DogModel dogModel);
}
