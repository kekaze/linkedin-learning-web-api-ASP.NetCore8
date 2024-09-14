using System.Data.Common;
using HPlusSport.API.Models;
using Microsoft.AspNetCore.Mvc;

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
        public ActionResult GetAllProducts()
        {
            return Ok(_context.Products.ToArray());
        }
        // [HttpGet, Route("api/[controller]/{id}")]
        [HttpGet("{id}")] // this is the same as the code above
        public Product GetProduct(int id)
        {
            return _context.Products.Find(id);
        }
    }
}
