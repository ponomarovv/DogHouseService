using AutoMapper;
using DogHouseService.Api.Mapping;
using DogHouseService.BLL.Interfaces;
using DogHouseService.BLL.Services;

namespace DogHouseService.Api.Extensions;

public static class BllDependencyInstaller
{
    public static void InstallServices(this IServiceCollection services)
    {
        services.AddScoped<IDogService, DogService>();

    }

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
