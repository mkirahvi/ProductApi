using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ProductsApi.Models;
using ProductsApi.Repositories;

namespace ProductsApi.Controllers
{
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        public IRepository<Product> ProductRepository { get; set; }

        public ProductController(IRepository<Product> productRepository)
        {
            ProductRepository = productRepository;
        }

        [HttpGet("/search")]
        public IEnumerable<Product> Get(string query = null)
        {
            return ProductRepository.GetAll(query);
        }

        [Authorize]
        [HttpGet("{id}", Name = "GetProduct")]
        public IActionResult GetProduct(string id)
        {
            var item = ProductRepository.Find(id);
            if(item == null)
            {
                return NotFound();
            }

            return new JsonResult(item);
        }
    }
}
