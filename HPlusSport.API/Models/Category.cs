using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HPlusSport.API.Models;

namespace HPlusSport.API.Models
{
    public class Category
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
    }
}