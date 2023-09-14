using LoncotesLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using loncotes_county_library.Migrations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// allows passing datetimes without time zone data 
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// allows our api endpoints to access the database through Entity Framework Core
builder.Services.AddNpgsql<LoncotesLibraryDbContext>(builder.Configuration["LoncotesLibraryDbConnectionString"]);

// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//endoints
app.MapGet("/api/materials", (LoncotesLibraryDbContext db, int? materialTypeId, int? genreId) =>
{
    List<Material> materials = db.Materials
    .Include(m => m.MaterialType)
    .Include(m => m.Genre)
    .Where(m => m.OutOfCirculationSince == null)
    .ToList();

    if (materialTypeId != null && genreId != null)
    {
        return Results.Ok(materials.Where(m => m.GenreId == genreId && m.MaterialTypeId == materialTypeId).ToList());
    }
    else if (materialTypeId != null && genreId == null)
    {
        return Results.Ok(materials.Where(m => m.MaterialTypeId == materialTypeId));
    }
    else if (materialTypeId == null && genreId != null)
    {
        return Results.Ok(materials.Where(m => m.GenreId == genreId));
    }
    return Results.Ok(materials);
});

app.MapGet("/api/materials/{id}", (LoncotesLibraryDbContext db, int id) =>
{
    return db.Materials
    .Include(m => m.Genre)
    .Include(m => m.MaterialType)
    .Include(m => m.Checkouts)
    .ThenInclude(c => c.Patron)
    .SingleOrDefault(m => m.Id == id);
});

app.MapPost("/api/materials", (LoncotesLibraryDbContext db, Material material) =>
{
    db.Materials.Add(material);
    db.SaveChanges();
    return Results.Created($"/api/materials/{material.Id}", material);
});

app.MapPut("/api/materials/{id}", (LoncotesLibraryDbContext db, int id) =>
{
    Material material = db.Materials.SingleOrDefault(m => m.Id == id);
    if (material == null){
        return Results.NotFound();
    }
    
    material.OutOfCirculationSince = DateTime.Now;
    db.SaveChanges();
    return Results.Ok();
});

// The librarians will need a form in their app that let's them choose material types. Create an endpoint that retrieves all of the material types to eventually populate that form field

app.MapGet("/api/materialTypes", (LoncotesLibraryDbContext db) =>
{
    return db.MaterialTypes;
});

// The librarians will also need form fields that have all of the genres to choose from. Create an endpoint that gets all of the genres.
app.MapGet("/api/genres", (LoncotesLibraryDbContext db) =>
{
    return db.Genres;
});

// The librarians want to see a list of library patrons.
app.MapGet("/api/patrons", (LoncotesLibraryDbContext db) =>
{

    return db.Patrons
    .Include(p => p.checkouts)
    .ThenInclude(c => c.Material)
    .ThenInclude(m => m.MaterialType);
});

// This endpoint should get a patron and include their checkouts, and further include the materials and their material types.
app.MapGet("/api/patrons/{id}", (LoncotesLibraryDbContext db, int id) =>
{
    return db.Patrons
    .Include(p => p.checkouts)
    .ThenInclude(c => c.Material)
    .ThenInclude(m => m.MaterialType)
    .SingleOrDefault(p => p.Id == id);
});
// Sometimes patrons move or change their email address. Add an endpoint that updates these properties only.
app.MapPut("/api/patrons/{id}/update", (LoncotesLibraryDbContext db, int id, Patron updatedPatron) =>
{
    //get the specific patron we are updating.
    Patron patron = db.Patrons.SingleOrDefault(p => p.Id == id);
    if (patron == null)
    {
        return Results.NotFound();
    }
    patron.Email = updatedPatron.Email;
    patron.Address = updatedPatron.Address;
    db.SaveChanges();
    return Results.Ok(patron);
});

app.MapPut("/api/patron/{id}/deactivate", (LoncotesLibraryDbContext db, int id) =>
{
    Patron patron = db.Patrons.SingleOrDefault(p => p.Id == id);
    if (patron == null)
    {
        return Results.NotFound();
    }
    patron.IsActive = false;
    db.SaveChanges();
    return Results.Ok(patron);
});
//activate accounts
app.MapPut("/api/patron/{id}/activate", (LoncotesLibraryDbContext db, int id) =>
{
    Patron patron = db.Patrons.SingleOrDefault(p => p.Id == id);
    if (patron == null)
    {
        return Results.NotFound();
    }
    patron.IsActive = true;
    db.SaveChanges();
    return Results.Ok(patron);
});

// The librarians need to be able to checkout items for patrons. Add an endpoint to create a new Checkout for a material and patron. Automatically set the checkout date to DateTime.Today.

app.MapPost("/api/checkouts", (LoncotesLibraryDbContext db, Checkout checkout) =>
{
    checkout.CheckoutDate = DateTime.Today;
    db.Checkouts.Add(checkout);
    db.SaveChanges();
    return Results.Created($"/api/checkouts/{checkout.Id}", checkout);
});

// The librarians need an endpoint to mark a checked out item as returned by item id. Add an endpoint expecting a checkout id, and update the checkout with a return date of DateTime.Today.
app.MapPut("/api/checkouts/{id}/returned", (LoncotesLibraryDbContext db, int id) =>
{
    Checkout checkout = db.Checkouts.SingleOrDefault(c => c.Id == id);
    if (checkout == null)
    {
        return Results.NotFound();
    }
    checkout.ReturnDate = DateTime.Today;
    db.SaveChanges();
    return Results.Ok(checkout);
});

//get a list of all the checkouts...
app.MapGet("/api/checkouts", (LoncotesLibraryDbContext db) =>
{
    return db.Checkouts;
});

//re read over these two endpoints.
app.MapGet("/materials/available", (LoncotesLibraryDbContext db) =>
{
    return db.Materials
    .Where(m => m.OutOfCirculationSince == null)
    .Where(m => m.Checkouts.All(co => co.ReturnDate != null))
    .ToList();
});

app.MapGet("/checkouts/overdue", (LoncotesLibraryDbContext db) =>
{
    return db.Checkouts
    .Include(p => p.Patron)
    .Include(co => co.Material)
    .ThenInclude(m => m.MaterialType)
    .Where(co =>
        (DateTime.Today - co.CheckoutDate).Days > 
        co.Material.MaterialType.CheckoutDays &&
        co.ReturnDate == null)
    .ToList();
});

app.Run();
