using System.Data.Common;
using HPlusSport.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HPlusSport.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ShopContext _context;

        public ProductsController(ShopContext context)
        {
            _context = context;

            _context.Database.EnsureCreated();
        }

        [HttpGet]
        public async Task<ActionResult> GetAllProducts([FromQuery]QueryParameters queryParameters)
        {
            IQueryable<Product> products = _context.Products;
            products = products
                .Skip(queryParameters.Size * (queryParameters.Page - 1))
                .Take(queryParameters.Size);

            return Ok(await products.ToArrayAsync());
        }
        // [HttpGet, Route("api/[controller]/{id}")]
        [HttpGet("{id}")] // this is the same as the code above
        public async Task<ActionResult> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpGet("available")]
        public async Task<ActionResult> GetOnStock()
        {
            var productsOnStock = await _context.Products.Where(x => x.IsAvailable).ToArrayAsync();
            return Ok(productsOnStock);
        }

        [HttpPost]
        public async Task<ActionResult> PostProduct(Product product)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
            nameof(GetProduct),
            new { id = product.Id },
            product);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutProduct([FromRoute] int id, [FromBody] Product product) // .Net is smart enough to know where an argument is coming from but it is a best practice to provide the attribute [FromRoute] and [FromBody] for a better code readability
        {
            if (id != product.Id) // if the id on the route is not the id on the payload
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) // looking for concurrency issues i.e. a parallel call changed something on the database for the same product
            {
                if (!_context.Products.Any(p => p.Id == id))
                {
                    return NotFound();
                } else
                {
                    throw; // server error
                }
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct([FromRoute] int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok(product);
        }
    }
}