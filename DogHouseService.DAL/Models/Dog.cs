using System.ComponentModel.DataAnnotations;

namespace DogHouseService.DAL.Models;

public class Dog
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    [Required]
    [StringLength(50)]
    public string Color { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int TailLength { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int Weight { get; set; }
}