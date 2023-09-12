using Microsoft.EntityFrameworkCore;
using LoncotesLibrary.Models;

public class LoncotesLibraryDbContext : DbContext
{

    public DbSet<Checkout> Checkouts { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Material> Materials { get; set; }
    public DbSet<MaterialType> MaterialTypes { get; set; }
    public DbSet<Patron> Patrons { get; set; }

    public LoncotesLibraryDbContext(DbContextOptions<LoncotesLibraryDbContext> context) : base(context)
    {

    }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<MaterialType>().HasData(new MaterialType[]{
      new MaterialType { Id = 1, Name = "book", CheckoutDays= 14},
      new MaterialType { Id = 2, Name = "CD", CheckoutDays= 8},
      new MaterialType { Id = 3, Name = "Map", CheckoutDays= 21},
    });

    modelBuilder.Entity<Patron>().HasData(new Patron[]{
      new Patron{ Id = 1, FirstName = "Caleb", LastName = "Sullivan", Address = "222 street", Email = "Caleb@Email.com", IsActive = true },
      new Patron{ Id = 2, FirstName = "Josh", LastName = "Dude", Address = "213 Ave", Email = "Josh@Email.com", IsActive = true }
    });

    modelBuilder.Entity<Genre>().HasData(new Genre[]{
    new Genre { Id = 1, Name = "Sci-fi"},
    new Genre { Id = 2, Name = "Horror"},
    new Genre { Id = 3, Name = "Music"},
    new Genre { Id = 4, Name = "Educational"},
    new Genre { Id = 5, Name = "Romance"},
  });

    modelBuilder.Entity<Material>().HasData(new Material[]{
      new Material {Id = 1, MaterialName = "David's Coffee Conspiracies", MaterialTypeId = 1, GenreId = 4 },
      new Material {Id = 2, MaterialName = "Romeo and Juliet", MaterialTypeId = 1, GenreId = 5 },
      new Material {Id = 3, MaterialName = "The Notebook", MaterialTypeId = 1, GenreId = 5 },
      new Material {Id = 4, MaterialName = "Jaws", MaterialTypeId = 1, GenreId = 2 },
      new Material {Id = 5, MaterialName = "David's buried Coffee Treasure!", MaterialTypeId = 3, GenreId = 4 },
      new Material {Id = 6, MaterialName = "History Textbook", MaterialTypeId = 1, GenreId = 4 },
      new Material {Id = 7, MaterialName = "Yard Sale ", MaterialTypeId = 2, GenreId = 3 },
      new Material {Id = 8, MaterialName = "Himalayas", MaterialTypeId = 2, GenreId = 3 },
      new Material {Id = 9, MaterialName = "Starfield", MaterialTypeId = 1, GenreId = 1 },
      new Material {Id = 10, MaterialName = "Scream", MaterialTypeId = 1, GenreId = 2 }
  });

    modelBuilder.Entity<Checkout>().HasData(new Checkout[]{
    new Checkout {Id = 1, MaterialId = 1, PatronId = 1, CheckoutDate =  new DateTime(2023,9,11)},
    new Checkout {Id = 2, MaterialId = 4, PatronId = 1, CheckoutDate =  new DateTime(2023,9,10)},
    new Checkout {Id = 3, MaterialId = 3, PatronId = 2, CheckoutDate =  new DateTime(2023,9,6)}
  });
 }
}