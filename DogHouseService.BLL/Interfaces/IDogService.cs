using System.Collections.Generic;
using System.Threading.Tasks;
using DogHouseService.BLL.Models;

namespace DogHouseService.BLL.Interfaces
{
    public interface IDogService
    {
        Task<IEnumerable<DogModel>> GetDogsAsync(string attribute = null, string order = null, int pageNumber = 1, int pageSize = 10);
        Task<DogModel> CreateDogAsync(DogModel dogModel);
    }
}
