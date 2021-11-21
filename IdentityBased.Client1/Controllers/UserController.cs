using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityBased.Client1.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            //ViewData["userName"] = User.Claims.Where(x => x.Type == ClaimTypes.Email).FirstOrDefault().Value;

            return View();
        }
    }
}