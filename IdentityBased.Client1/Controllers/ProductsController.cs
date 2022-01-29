using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityBased.Client1.Models;
using IdentityBased.Client1.Services;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;

namespace IdentityBased.Client1.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        List<Product> products = new List<Product>();
        private readonly IConfiguration _configuration;
        private readonly IApiHttpClient _apiHttpClient;

        public ProductsController(IConfiguration configuration, IApiHttpClient apiHttpClient)
        {
            _configuration = configuration;
            _apiHttpClient = apiHttpClient;
        }
        public async Task<IActionResult> Index()
        {
            var userName = User.Claims.First(x => x.Type == "name");
            //*LAST STEP
            HttpClient client = await _apiHttpClient.GetHttpClient();
            List<Product> products = new List<Product>();
            //NO NEED WITH SERVICE
            //*SECOND STEP
            //HttpClient httpClient = new HttpClient();           
            //var accessToken  = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            //var disco = await httpClient.GetDiscoveryDocumentAsync("https://localhost:5001");
            
            //*FIRST STEP
            //ClientCredentialsTokenRequest clientCredentialsTokenRequest = new ClientCredentialsTokenRequest();
            //clientCredentialsTokenRequest.ClientId = _configuration["Client:ClientId"];
            //clientCredentialsTokenRequest.ClientSecret = _configuration["Client:ClientSecret"];
            //clientCredentialsTokenRequest.Address = disco.TokenEndpoint;
            //var token = await httpClient.RequestClientCredentialsTokenAsync(clientCredentialsTokenRequest);

            //if (token.IsError)
            //{
            //    //Loggin
            //}
            ////Client1: https://localhost:5003

            //httpClient.SetBearerToken(token.AccessToken); //Without SetBearerToken: httpClient.DefaultRequestHeader.Authorization = new .....
            //httpClient.SetBearerToken(accessToken); //Without SetBearerToken: httpClient.DefaultRequestHeader.Authorization = new .....
            
            var response = await client.GetAsync("https://localhost:44369/api/products/getproducts"); //TODO: New Interface with a class to access. PUT POST DELETE

            //await httpClient.PostAsync();
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(); // Trigger to check data https://localhost:5003/products
                products = JsonConvert.DeserializeObject<List<Product>>(content);
            }
            else
            {
                //logging
            }

            return View(products);
        }
    }
}