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

        public UserController(IUserRepository userRepository)
        {
            UserRepository = userRepository;
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
            await Response.WriteAsync( JsonConvert.SerializeObject( response, new JsonSerializerSettings { Formatting = Formatting.Indented } ) );
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
        [HttpGet("/products")]
        public IEnumerable<MonitoredProduct> GetMonitoredProducts()
        {
            var user = UserRepository
                .GetAll()
                .FirstOrDefault( x => x.Name == User.Identity.Name );

            if(user == null )
            {
                return Enumerable.Empty<MonitoredProduct>();
            }

            return user.MonitoredProducts;
        }

        [HttpGet]
        public IEnumerable<User> Get()
        {
            return UserRepository.GetAll();
        }

        [HttpGet("{id}", Name = "GetUser")]
        public IActionResult GetUser(string id)
        {
            var item = UserRepository.Find(id);
            if(item == null )
            {
                return NotFound();
            }

            return new JsonResult( item );
        }

        [HttpPost]
        public IActionResult Create([FromBody]User item)
        {
            if( item == null )
            {
                return BadRequest();
            }
            UserRepository.Add( item );
            return CreatedAtRoute( "GetUser", new { id = item.Id.ToString() }, item );
        }

        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody]User item)
        {
            if( item == null || item.Id != id )
            {
                return BadRequest();
            }

            var todo = UserRepository.Find( id );
            if( todo == null )
            {
                return NotFound();
            }

            UserRepository.Update( item );
            return new NoContentResult();

        }

        [HttpDelete( "{id}" )]
        public IActionResult Delete( string id )
        {
            var todo = UserRepository.Find( id );
            if( todo == null )
            {
                return NotFound();
            }

            UserRepository.Remove( id );
            return new NoContentResult();
        }
    }
}
