using IdentityBased.Client1.Models;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IdentityBased.Client1.Services
{
    public class ApiResourcesHttpClient : IApiHttpClient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private HttpClient _client;

        public ApiResourcesHttpClient(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _client = new HttpClient();
        }
        public async Task<HttpClient> GetHttpClient()
        {
            var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            _client.SetBearerToken(accessToken);
            return _client;
        }

        public async Task<List<string>> SaveUserViewModel(UserRecordViewModel userRecordViewModel)
        {
            var disco = await _client.GetDiscoveryDocumentAsync(_configuration["ServerUrl"]);
            if(disco.IsError)
            {
                //You coulndt reach identity at port 5001.
            }
            var clientCredintials = new ClientCredentialsTokenRequest();
            clientCredintials.ClientId = _configuration["ClilentResourceOwner:ClientId"];
            clientCredintials.ClientSecret = _configuration["ClilentResourceOwner:ClientSecret"];
            clientCredintials.Address = disco.TokenEndpoint;

            var token = await _client.RequestClientCredentialsTokenAsync(clientCredintials);

            if(token.IsError)
            {
                //
            }
            var stringContent = new StringContent(JsonConvert.SerializeObject(userRecordViewModel), Encoding.UTF8, "application/json");
            _client.SetBearerToken(token.AccessToken);
            var response = await _client.PostAsync("https://localhost:5001/api/user/signup", stringContent);

            if(!response.IsSuccessStatusCode)
            {
                var errorsList = JsonConvert.DeserializeObject<List<string>> (await response.Content.ReadAsStringAsync());

                return errorsList;
            }

            return null;


        }
    }
}
