using AutoMapper;
using DogHouseService.Api.Mapping;

namespace DogHouseService.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void InstallMappers(this IServiceCollection services)
    {
        var config = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new MappingProfile());
        });

        IMapper mapper = config.CreateMapper();
        services.AddSingleton(mapper);
    }
}
