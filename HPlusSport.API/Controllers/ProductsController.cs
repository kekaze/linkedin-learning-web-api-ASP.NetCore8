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
        public async Task<ActionResult> GetAllProducts()
        {
            return Ok(await _context.Products.ToArrayAsync());
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
    }
}