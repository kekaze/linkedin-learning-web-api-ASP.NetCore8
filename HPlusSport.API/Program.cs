using HPlusSport.API.Models;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ShopContext>(options =>
{
    options.UseInMemoryDatabase("Shop");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope()) 
{
    var db = scope.ServiceProvider.GetRequiredService<ShopContext>();
    await db.Database.EnsureCreatedAsync();
}

app.MapGet("/products", async (ShopContext _context) =>
{
    return await _context.Products.ToArrayAsync();
});

app.MapGet("/products/{id}", async (int id, ShopContext _context) =>
{
    var product = await _context.Products.FindAsync();
    if (product == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(product);
}).WithName("GetProduct");

app.MapGet("/products/available", async (ShopContext _context) =>
{
    return await _context.Products.Where(product => product.IsAvailable).ToArrayAsync();
});

app.MapPost("/products", async (ShopContext _context, Product product) =>
{
    if (string.IsNullOrEmpty(product.Name)) // custom vallidation
    {
        return Results.BadRequest("Product name is required.");
    }

    _context.Products.Add(product);
    await _context.SaveChangesAsync();

    return Results.CreatedAtRoute(
    "GetProduct",
    new { id = product.Id },
    product);
});

app.MapPut("/products", async (int id, ShopContext _context, Product product) =>
{
    if (id != product.Id) // if the id on the route is not the id on the payload
    {
        return Results.BadRequest();
    }

    _context.Entry(product).State = EntityState.Modified;

    try
    {
        await _context.SaveChangesAsync();
    } catch (DbUpdateConcurrencyException) // looking for concurrency issues i.e. a parallel call changed something on the database for the same product
    {
        if (!_context.Products.Any(p => p.Id == id))
        {
            return Results.NotFound();
        } else
        {
            throw; // server error
        }
    }
    return Results.NoContent();
});
app.MapDelete("/products/{id}", async (int id, ShopContext _context) =>
{
    var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return Results.NotFound();
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Results.Ok(product);
});

app.Run();
