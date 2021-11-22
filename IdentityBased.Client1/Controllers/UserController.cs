using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace IdentityBased.Client1.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            //ViewData["userName"] = User.Claims.Where(x => x.Type == ClaimTypes.Email).FirstOrDefault().Value;
            return View();
        }



        public async Task LogOut()
        {
            await HttpContext.SignOutAsync("Cookies"); //Logout from client1
            await HttpContext.SignOutAsync("oidc");  //Logout from IdentiyServer
            //return from IdentityServer
        }

        public async Task<IActionResult> GetRefreshToken()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            HttpClient httpClient = new HttpClient(clientHandler);
            var disco = await httpClient.GetDiscoveryDocumentAsync("https://localhost:5001");
            if (disco.IsError)
            {
                //Logging
            }
            var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            RefreshTokenRequest refreshTokenRequest = new RefreshTokenRequest();
            refreshTokenRequest.ClientId = _configuration["ClientMVC:ClientId"];
            refreshTokenRequest.ClientSecret = _configuration["ClientMVC:ClientSecret"];
            refreshTokenRequest.RefreshToken = refreshToken;
            refreshTokenRequest.Address = disco.TokenEndpoint;//We don't give it Username pw; bcs IdentityServer recognize user from coookkkiieee
            var token = await httpClient.RequestRefreshTokenAsync(refreshTokenRequest);//WTF
            if (token.IsError)
            {
                //Log and Redirect wherever u want 
            }
            var tokens = new List<AuthenticationToken>()
            {
                new AuthenticationToken{Name = OpenIdConnectParameterNames.IdToken,Value=token.IdentityToken},
                new AuthenticationToken{Name = OpenIdConnectParameterNames.AccessToken,Value=token.AccessToken},
                new AuthenticationToken{Name = OpenIdConnectParameterNames.RefreshToken,Value=token.RefreshToken},
                new AuthenticationToken{Name = OpenIdConnectParameterNames.ExpiresIn,Value=DateTime.UtcNow.AddSeconds(token.ExpiresIn).ToString("o",CultureInfo.InvariantCulture)}
            };

            var authenticationResult = await HttpContext.AuthenticateAsync();
            var properties = authenticationResult.Properties;
            properties.StoreTokens(tokens);
            await HttpContext.SignInAsync("Cookies", authenticationResult.Principal, properties);//StartupCS: Updated User Cookie/Token information

            return RedirectToAction("Index");
        }
    }
    
} 