using IdentityBased.Client1.Models;
using IdentityBased.Client1.Services;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityBased.Client1.Controllers
{
    public class LoginController : Controller
    {
        IConfiguration _configuration;
        private readonly IApiHttpClient _apiHttpClient;

        public LoginController(IConfiguration configuration, IApiHttpClient apiHttpClient)
        {
            _configuration = configuration;
            _apiHttpClient = apiHttpClient;
        }
        public IActionResult Index() 
        {
            return View();
        }
        
        [HttpPost]
        public async Task <IActionResult> Index(LoginViewModel loginViewModel) 
        {
            //Token-> UserInfo-> Claim
            var client = new HttpClient();

            var disco = await client.GetDiscoveryDocumentAsync(_configuration["ServerUrl"]);

            if(disco.IsError)
            {
                //logging and stuff
            }
            var password = new PasswordTokenRequest();  
              
            password.Address = disco.TokenEndpoint;
            password.UserName = loginViewModel.Email;
            password.Password = loginViewModel.Password;
            password.ClientId = _configuration["ClilentResourceOwner:ClientId"];
            password.ClientSecret= _configuration["ClilentResourceOwner:ClientSecret"];

            var token = await client.RequestPasswordTokenAsync(password);



            if(token.IsError)
            {
                ModelState.AddModelError("", "Email or password is wrong");
                return View();
                //token.Error
            }

            var userInfoRequest = new UserInfoRequest();
            userInfoRequest.Token = token.AccessToken;
            userInfoRequest.Address = disco.UserInfoEndpoint;
            var userInfo = await client.GetUserInfoAsync(userInfoRequest);


            if (userInfo.IsError)
            {
                //token.Error
            }
            //Which role which name for claim
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(userInfo.Claims, CookieAuthenticationDefaults.AuthenticationScheme,"name","role");
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            //Lets Sign-in
            var authenticationProperties = new AuthenticationProperties();
            //Cookie informations
            authenticationProperties.StoreTokens(new List<AuthenticationToken>()
            {
               // new AuthenticationToken{Name = OpenIdConnectParameterNames.IdToken,Value=token.IdentityToken},
                new AuthenticationToken{Name = OpenIdConnectParameterNames.AccessToken,Value=token.AccessToken},
                new AuthenticationToken{Name = OpenIdConnectParameterNames.RefreshToken,Value=token.RefreshToken},
                new AuthenticationToken{Name = OpenIdConnectParameterNames.ExpiresIn,Value=DateTime.UtcNow.AddSeconds(token.ExpiresIn).ToString("o",CultureInfo.InvariantCulture)}
            });

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal,authenticationProperties);

            return RedirectToAction("Index", "User");
        }

      
        public IActionResult SignUp()
        {
            return View();
        } 
        
        [HttpPost]
        public async Task<IActionResult> SignUp(UserRecordViewModel userRecordViewModel)
        {
            if (!ModelState.IsValid) return View();

            var result = await _apiHttpClient.SaveUserViewModel(userRecordViewModel);

            if(result != null)
            {
                result.ForEach(error =>
                {
                    ModelState.AddModelError("", error);
                });
                return View();
            }

            return RedirectToAction("Index");
        }



    }
}
