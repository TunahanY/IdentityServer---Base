using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityBased.API1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityBased.API1.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        // /api/products/GetProducts
        [Authorize]
        [HttpGet]
        public IActionResult GetProducts()
        {
            var productList = new List<Product>()
            { 
                new Product { Id = 1, Name = "Keyboard", Price = 100, Stock = 10 }, 
                new Product { Id = 2, Name = "Doll", Price = 200, Stock = 20 }, 
                new Product { Id = 3, Name = "Mouse", Price = 300, Stock = 30 }, 
                new Product { Id = 4, Name = "Bike", Price = 400, Stock = 40 } 
            };

            return Ok(productList);
        }
    }
}