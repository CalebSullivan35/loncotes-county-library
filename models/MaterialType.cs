using System.ComponentModel.DataAnnotations;
namespace LoncotesLibrary.Models;

public class MaterialType {
  public int Id { get; set; } 
  public string Name { get; set; } [Required]
  public int CheckoutDays { get; set; } 
}