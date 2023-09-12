using System.ComponentModel.DataAnnotations;
namespace LoncotesLibrary.Models;

public class Material {
  public int Id { get; set; }
  public string MaterialName { get; set; } [Required]
  public int MaterialTypeId { get; set; }
  public MaterialType MaterialType { get; set; }
  public int GenreId { get; set; }
  public Genre Genre { get; set; }
  public DateTime? OutOfCirculationSince {get; set;}
  public List<Checkout>? Checkouts { get; set; }
}