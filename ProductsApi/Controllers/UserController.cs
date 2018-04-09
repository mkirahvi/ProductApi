using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using ProductsApi.Models;
using ProductsApi.Repositories;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProductsApi.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        public IUserRepository UserRepository { get; set; }
        public IProductRepository ProductRepository { get; set; }

        public UserController(IUserRepository userRepository, IProductRepository productRepository )
        {
            UserRepository = userRepository;
            ProductRepository = productRepository;
        }

        [HttpPost( "/token" )]
        public async Task Token()
        {
            var username = Request.Form["username"];
            var password = Request.Form["password"];

            var identity = GetIdentity( username, password );
            if( identity == null )
            {
                Response.StatusCode = 400;
                await Response.WriteAsync( "Invalid username or password." );
                return;
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add( TimeSpan.FromMinutes( AuthOptions.LIFETIME ) ),
                    signingCredentials: new SigningCredentials( AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256 ) );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken( jwt );
            
            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };

            // сериализация ответа
            Response.ContentType = "application/json";
            await Response.WriteAsync( JsonConvert.SerializeObject( encodedJwt, new JsonSerializerSettings { Formatting = Formatting.None } ) );
        }

        private ClaimsIdentity GetIdentity( string username, string password )
        {
            User person = UserRepository
                    .GetAll()
                    .FirstOrDefault( x => x.Name == username && x.Password == password );

            if( person != null )
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, person.Name)
                };

                ClaimsIdentity claimsIdentity = 
                    new ClaimsIdentity( claims, "Token", 
                            ClaimsIdentity.DefaultNameClaimType,
                            ClaimsIdentity.DefaultRoleClaimType );
                
                return claimsIdentity;
            }

            // если пользователя не найдено
            return null;
        }

        [Authorize]
        [HttpGet( "/products" )]
        public IEnumerable<MonitoredProduct> GetMonitoredProducts()
        {
            var user = UserRepository
                .GetAll()
                .FirstOrDefault( x => x.Name == User.Identity.Name );

            if( user == null || user.MonitoredProducts == null )
            {
                return Enumerable.Empty<MonitoredProduct>();
            }

            var result = user.MonitoredProducts;
            foreach( var item in result )
            {
                item.Product = ProductRepository.Find( item.ProductId );
            }

            return result;
        }

        /// <summary>
        /// http://localhost:7575/subscribe?productId=5a14a7d129c52c6442372045
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("/subscribe")]
        public IActionResult Create(string productId)
        {
            if( string.IsNullOrWhiteSpace(productId) )
            {
                return BadRequest();
            }

            var user = UserRepository
                .GetAll()
                .FirstOrDefault( x => x.Name == User.Identity.Name );

            if(user.MonitoredProducts != null && user.MonitoredProducts.Any(x => x.ProductId.Equals(productId, StringComparison.InvariantCultureIgnoreCase)))
            {
                return Content("Already subscribed");
            }

            MonitoredProduct p = new MonitoredProduct
            {
                ProductId = productId,
                NotificationSettings = new NotificationSettings
                {
                    Availability = true,
                    PriceChanging = true
                }
            };

            user.MonitoredProducts.Add( p );
            UserRepository.Update( user);
            return Content( "Done" );
        }

        /// <summary>
        /// http://localhost:7575/unsubscribe?productId=5a14a7d129c52c6442372045
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost( "/unsubscribe" )]
        public IActionResult Delete( string productId )
        {
            if( string.IsNullOrWhiteSpace( productId ) )
            {
                return BadRequest();
            }

            var user = UserRepository
                .GetAll()
                .FirstOrDefault( x => x.Name == User.Identity.Name );

            var p = user.MonitoredProducts?.FirstOrDefault( x => x.ProductId.Equals( productId, StringComparison.InvariantCultureIgnoreCase ) );
            if( p == null)
            {
                return Content( "No such subscription" );
            }

            user.MonitoredProducts.Remove( p );

            UserRepository.Update( user );
            return new NoContentResult();
        }
    }
}
