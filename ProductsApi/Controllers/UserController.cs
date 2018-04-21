using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using ProductsApi.Models;
using ProductsApi.Repositories;

namespace ProductsApi.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        public IRepository<User> UserRepository { get; set; }
        public IRepository<Product> ProductRepository { get; set; }

        public UserController(IRepository<User> userRepository, IRepository<Product> productRepository)
        {
            UserRepository = userRepository;
            ProductRepository = productRepository;
        }

        [HttpPost("/token")]
        public IActionResult Token()
        {
            var username = Request.Form["username"];
            var password = Request.Form["password"];

            var identity = GetIdentity(username, password);
            if(identity == null)
            {
                Response.StatusCode = 400;
                return Content("Invalid username or password");
            }

            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };

            return Content(encodedJwt);
        }

        private ClaimsIdentity GetIdentity(string username, string password)
        {
            User person = UserRepository
                    .GetAll(username)
                    .FirstOrDefault(x => x.Password == password);

            if(person != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, person.Name)
                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

                return claimsIdentity;
            }

            return null;
        }

        [Authorize]
        [HttpGet("/products")]
        public IEnumerable<MonitoredProduct> GetMonitoredProducts()
        {
            var user = getCurrentUser();

            if(user == null || user.MonitoredProducts == null)
            {
                return Enumerable.Empty<MonitoredProduct>();
            }

            var result = user.MonitoredProducts;
            foreach(var item in result)
            {
                item.Product = ProductRepository.Find(item.ProductId);
            }

            return result;
        }

        [Authorize]
        [HttpPost("/subscribe")]
        public IActionResult Create(string productId)
        {
            if(string.IsNullOrWhiteSpace(productId))
            {
                return BadRequest();
            }

            var user = getCurrentUser();

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

            user.MonitoredProducts.Add(p);
            UserRepository.Update(user);
            return Content("Done");
        }

        [Authorize]
        [HttpPost("/unsubscribe")]
        public IActionResult Delete(string productId)
        {
            if(string.IsNullOrWhiteSpace(productId))
            {
                return BadRequest();
            }

            var user = getCurrentUser();

            var p = user.MonitoredProducts?.FirstOrDefault(x => x.ProductId.Equals(productId, StringComparison.InvariantCultureIgnoreCase));
            if(p == null)
            {
                return Content("No such subscription");
            }

            user.MonitoredProducts.Remove(p);

            UserRepository.Update(user);
            return new NoContentResult();
        }

        private User getCurrentUser()
        {
            return UserRepository
                .GetAll()
                .FirstOrDefault(x => x.Name == User.Identity.Name);
        }
    }
}
