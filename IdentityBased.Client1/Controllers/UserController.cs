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
            var userName = User.Claims.First(x => x.Type == "name").Value;
            var userName2 = User.Identity.Name;//??reachable with claims..
            var userName3 = HttpContext.User.Identity.Name; //Check
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
            refreshTokenRequest.RefreshToken = refreshToken;//Gotta Check2
            refreshTokenRequest.Address = disco.TokenEndpoint;//We don't give it Username pw; bcs IdentityServer recognize user from coookkkiieee
            var token = await httpClient.RequestRefreshTokenAsync(refreshTokenRequest);//WTF Gotta Check1
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
        
        //Sample TOKEN to Check on JWT.io
        //eyJhbGciOiJSUzI1NiIsImtpZCI6IkJERjhEODRDMjNERUI4MUYxRkEwRTAwQzJBMTU0RTA1IiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE2NDIyNzczNzUsImV4cCI6MTY0MjI4NDU3NSwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NTAwMSIsImF1ZCI6InJlc291cmNlX2FwaTEiLCJjbGllbnRfaWQiOiJDbGllbnQxLU12YyIsInN1YiI6IjIiLCJhdXRoX3RpbWUiOjE2NDIyNzczNzQsImlkcCI6ImxvY2FsIiwianRpIjoiREY5NDAzRTg2NzkwM0JFNTgwRUIwRjk4QzJBQ0FCREYiLCJzaWQiOiJGMTIyOTNFQUM5NURGM0M0QjlDMTJEODM3NjIzQzgwOSIsImlhdCI6MTY0MjI3NzM3NSwic2NvcGUiOlsib3BlbmlkIiwicHJvZmlsZSIsImFwaTEucmVhZCIsIkNvdW50cnlBbmRDaXR5IiwiUm9sZXMiLCJvZmZsaW5lX2FjY2VzcyJdLCJhbXIiOlsicHdkIl19.EmbSia11Py6TlMSqLZJHFspavuJvg0nUlyOTy7l4_YF-4rmtHyfT_Ur5J4aZhYQ1S786EYw39gZSMjpElWpxSaSlY4p331jypB6Lbu0Nic9kCqA4M2LSY3kC-kmkEvPQZ2I2oNOD3-Wi7Lcj4JggvoTKj6rkEuNbZoC-Y52hTm1yhtBNIiwVu63YD_M63QYrHGd68f3aCqkImmaH4F-6sS9bDFTAea29iO0NyAMb-6QoJh4ugOD7rJOj3byg_QsNPmbIb0jHJwvz-19cdBS2Gq_BsNZbWjr3vVp599aNxvdXKTduffqu-pop9ZCzefNbU7vQfNbIVYcJ6tIImi2a2Q


        //Need to logout first, bcs token have roles YET
        [Authorize(Roles ="admin")]
        public IActionResult AdminAction() 
        {
            return View();
        }  
        [Authorize(Roles ="admin,customer")]
        public IActionResult CustomerAction() 
        {
            return View();
        }

    }
    

} 