using AutoMapper;
using DogHouseService.BLL.Models;
using DogHouseService.DAL.Models;
using DogHouseService.Api.Models;

namespace DogHouseService.Api.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Dog, DogModel>().ReverseMap();
            CreateMap<DogModel, DogDto>().ReverseMap();
        }
    }
}
