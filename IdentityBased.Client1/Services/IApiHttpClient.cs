using IdentityBased.Client1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace IdentityBased.Client1.Services
{
    public interface IApiHttpClient
    {
        Task<HttpClient> GetHttpClient();
        Task<List<string>> SaveUserViewModel(UserRecordViewModel userRecordViewModel);
    }
}
