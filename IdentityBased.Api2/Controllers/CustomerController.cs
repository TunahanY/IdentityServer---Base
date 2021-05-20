using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityBased.API2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityBased.API2.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public IActionResult GetCustomers()
        {
            var customerList = new List<Customer>()
            {
                new Customer {Id=1,Name="Tunahan",Address="Turkey"},
                new Customer {Id=2,Name="Jôao",Address="Brasil"},
                new Customer {Id=3,Name="Barbara",Address="Portuguese"}
            };

            return Ok(customerList);
        }
    }
}