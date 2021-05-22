using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityBased.Client1.Models;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace IdentityBased.Client1.Controllers
{
    public class ProductsController : Controller
    {
        List<Product> products = new List<Product>();
        private readonly IConfiguration _configuration;

        public ProductsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IActionResult> Index()
        {
            HttpClient httpClient = new HttpClient();
            var disco = await httpClient.GetDiscoveryDocumentAsync("https://localhost:5001");
            if (disco.IsError)
            {
                //Logging
            }
            ClientCredentialsTokenRequest clientCredentialsTokenRequest = new ClientCredentialsTokenRequest();
            clientCredentialsTokenRequest.ClientId = _configuration["Client:ClientId"];
            clientCredentialsTokenRequest.ClientSecret = _configuration["Client:ClientSecret"];
            clientCredentialsTokenRequest.Address = disco.TokenEndpoint;
            var token = await httpClient.RequestClientCredentialsTokenAsync(clientCredentialsTokenRequest);

            if (token.IsError)
            {
                //Loggin
            }
            //Client1: https://localhost:5003

            httpClient.SetBearerToken(token.AccessToken); //Without SetBearerToken: httpClient.DefaultRequestHeader.Authorization = new .....

            var response = await httpClient.GetAsync("https://localhost:44369/api/products/getproducts");
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