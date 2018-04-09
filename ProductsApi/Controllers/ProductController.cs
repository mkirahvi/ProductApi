using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductsApi.Models;
using ProductsApi.Repositories;

namespace ProductsApi.Controllers
{
    [Route( "api/[controller]" )]
    public class ProductController : Controller
    {
        public IProductRepository ProductRepository { get; set; }

        public ProductController( IProductRepository productRepository )
        {
            ProductRepository = productRepository;
        }

        /// <summary>
        /// http://localhost:7575/search?query=sony
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet ("/search")]
        public IEnumerable<Product> Get( string query = null )
        {
            return ProductRepository.GetAll(query);
        }

        [Authorize]
        [HttpGet("{id}", Name = "GetProduct")]
        public IActionResult GetProduct(string id)
        {
            var item = ProductRepository.Find(id);
            if(item == null )
            {
                return NotFound();
            }

            return new JsonResult( item );
        }

        [HttpPost]
        public IActionResult Create([FromBody]Product item)
        {
            if( item == null )
            {
                return BadRequest();
            }
            ProductRepository.Add( item );
            return CreatedAtRoute( "GetProduct", new { id = item.Id.ToString() }, item );
        }

        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody]Product item)
        {
            if( item == null || item.Id != id )
            {
                return BadRequest();
            }

            var todo = ProductRepository.Find( id );
            if( todo == null )
            {
                return NotFound();
            }

            ProductRepository.Update( item );
            return new NoContentResult();

        }

        [HttpDelete( "{id}" )]
        public IActionResult Delete( string id )
        {
            var todo = ProductRepository.Find( id );
            if( todo == null )
            {
                return NotFound();
            }

            ProductRepository.Remove( id );
            return new NoContentResult();
        }
    }
}
