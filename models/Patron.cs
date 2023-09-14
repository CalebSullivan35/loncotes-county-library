using System.ComponentModel.DataAnnotations;
namespace LoncotesLibrary.Models;
public class Patron{
  public int Id { get; set; }
  public string FirstName { get; set; } [Required]
  public string LastName { get; set; } [Required]
  public string Address { get; set; } [ Required]
  public string Email { get; set; } [ Required]
  public bool IsActive { get; set; }
  public List<Checkout> checkouts { get; set;}

  public decimal? Balance 
  {
    get
    {
      if (checkouts == null || !checkouts.Any())
        {
            return null; // Return null if there are no checkouts or checkouts is null
        }
      // declare value
      decimal? total = 0M;
      foreach (Checkout checkout in checkouts){
      // check if the checkout is not paid
        if (checkout.Paid == false)
        {
          //add it to the total.
          total += checkout.LateFee;
        }
      }
      return total;
    }
  }
};