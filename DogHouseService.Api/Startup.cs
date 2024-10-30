// using Microsoft.AspNetCore.Builder;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Hosting;
// using DogHouseService.BLL.Interfaces;
// using DogHouseService.BLL.Services;
// using DogHouseService.DAL.Data;
// using AspNetCoreRateLimit;
// using AutoMapper;
// using DogHouseService.Api.Mapping;
//
// namespace DogHouseService.Api
// {
//     public class Startup
//     {
//         public Startup(IConfiguration configuration)
//         {
//             Configuration = configuration;
//         }
//
//         public IConfiguration Configuration { get; }
//
//         public void ConfigureServices(IServiceCollection services)
//         {
//             services.AddControllers();
//
//             services.AddDbContext<DogHouseContext>(options =>
//                 options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
//
//             services.AddScoped<IDogService, DogService>();
//
//             services.AddMemoryCache();
//             services.Configure<IpRateLimitOptions>(Configuration.GetSection("RateLimiting"));
//             services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
//             services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
//             services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
//
//             services.AddAutoMapper(typeof(MappingProfile));
//
//             services.AddSwaggerGen();
//         }
//
//         public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
//         {
//             if (env.IsDevelopment())
//             {
//                 app.UseDeveloperExceptionPage();
//             }
//
//             app.UseHttpsRedirection();
//
//             app.UseRouting();
//
//             app.UseAuthorization();
//
//             app.UseIpRateLimiting();
//
//             app.UseEndpoints(endpoints =>
//             {
//                 endpoints.MapControllers();
//             });
//
//             app.UseSwagger();
//             app.UseSwaggerUI(c =>
//             {
//                 c.SwaggerEndpoint("/swagger/v1/swagger.json", "DogHouseService API V1");
//             });
//         }
//     }
// }
