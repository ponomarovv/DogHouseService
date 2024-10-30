using Microsoft.EntityFrameworkCore;
using DogHouseService.DAL.Models;

namespace DogHouseService.DAL.Data;

public class DogHouseContext : DbContext
{
    public DogHouseContext()
    {
    }

    public DogHouseContext(DbContextOptions<DogHouseContext> options) : base(options)
    {
    }

    public DbSet<Dog> Dogs { get; set; }
}
