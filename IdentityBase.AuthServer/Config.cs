using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityBased.AuthServer
{
    public static class Config
    {
        //Which API's?
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>()
            {
                //IdentityServer will know which api has which authorization bcs of those guys.
                new ApiResource("resource_api1"){Scopes={"api1.read","api1.write","api1.update" } }, //Used in API1 for JWT
                new ApiResource("resource_api2"){Scopes={"api2.read","api2.write","api2.update" } }
            };
        }
        //With which skills?
        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>()
            {
                //Api1
                new ApiScope("api1.read","Read permission for API-1"),
                new ApiScope("api1.write","Write permission for API-1"),
                new ApiScope("api1.update","Update permission for API-1"),
                //Api2
                new ApiScope("api2.read","Read permission for API-2"),
                new ApiScope("api2.write","Write permission for API-2"),
                new ApiScope("api2.update","Update permission for API-2")
            };
        }

        //Which clients will use these api's?
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>()
            {
                new Client()
                {
                    ClientId ="Client1",
                    ClientName= "Tunahan Client1 WebApp",
                    ClientSecrets =new[]{new Secret("secret".Sha256())}, //Our key with SHA256
                    
                    //We don't need to know who is the client..
                    //This is why we choose ClientCredentials. Token won't have something about client.
                    //Just permissions about connection between API and Client
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = {"api1.read"}
                },
                new Client()
                {
                    ClientId ="Client2",
                    ClientName= "Tunahan Client2 WebApp",
                    ClientSecrets =new[]{new Secret("secret".Sha256())},
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = {"api1.read","api2.write","api2.update"}
                }
            };
        }
    }
}
