﻿using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
                new ApiResource("resource_api1"){
                    Scopes={"api1.read","api1.write","api1.update" },
                    ApiSecrets = new[]{new Secret("secretapi1".Sha256())} //Introspection Endpoint - WillDO! Move appsetting.json
                }, //Used in API1 for JWT
                new ApiResource("resource_api2"){
                    Scopes={"api2.read","api2.write","api2.update" },
                    ApiSecrets = new[]{new Secret("secretapi2".Sha256())} //Our password for Introspection Endpoint: To check is token valid for API || Token parsing
                }
            };
        }
        //With which skills?
        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>()
            {
                //API1
                new ApiScope("api1.read","Read permission for API-1"),
                new ApiScope("api1.write","Write permission for API-1"),
                new ApiScope("api1.update","Update permission for API-1"),
                //API2
                new ApiScope("api2.read","Read permission for API-2"),
                new ApiScope("api2.write","Write permission for API-2"),
                new ApiScope("api2.update","Update permission for API-2")
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>()
            {
                new IdentityResources.Email(),
                new IdentityResources.OpenId(), //UsersId -> HAVE TO BE IN TOKEN -SubId
                new IdentityResources.Profile(),
                 // Custom identity.
                new IdentityResource(){Name="CountryAndCity",DisplayName = "Country and city",Description = "Users country and city info", //We'll see the permission view
                UserClaims = new []{"country","city"}},


                //Role based Authentication
                new IdentityResource(){Name="Roles",DisplayName="Roles",Description="User roles",UserClaims= new[] {"role"}}
            };
        }

        public static IEnumerable<TestUser> GetUsers()
        {
            return new List<TestUser>()
            {
                new TestUser{SubjectId = "1", Username="tyollar",Password="password",Claims= new List<Claim>(){
                    new Claim("given_name","Tonyhan"),
                    new Claim("family_name","Yollar"),
                    new Claim("country","Turkey"),
                    new Claim("city","Istanbul"),
                    new Claim("role","admin")
                }},
                new TestUser{SubjectId = "2", Username="tName",Password="tpass",Claims= new List<Claim>(){
                    new Claim("given_name","TestName"),
                    //new Claim("middle_name","middleName"),
                    new Claim("family_name","TestLastName"),
                     new Claim("country","Turkey"),
                    new Claim("city","Ankara"),
                    new Claim("role","customer")
                }},
                new TestUser{SubjectId = "3", Username="newUser",Password="password",Claims= new List<Claim>(){
                    new Claim("given_name","Hello"),
                    new Claim("family_name","Again")
                }
                }
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
                },
                new Client()
                {
                    ClientId ="Client1-Mvc",
                    RequirePkce=false,
                    ClientName= "Client1 MvcApp",
                    ClientSecrets =new[]{new Secret("secret".Sha256())},
                    AllowedGrantTypes= GrantTypes.Hybrid,
                    RedirectUris = new List<string>{ "https://localhost:5003/signin-oidc" }, //Protocol
                    PostLogoutRedirectUris = new List<string>{"https://localhost:5003/signout-callback-oidc"}, //Protocol - same in AuthServer
                    AllowedScopes = {IdentityServerConstants.StandardScopes.Email, IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile,"api1.read",
                        IdentityServerConstants.StandardScopes.OfflineAccess,"CountryAndCity","Roles" },
                    AccessTokenLifetime = 2*60*60,//DateTime.Now.AddHours(2).Second,
                    AllowOfflineAccess = true,//Refresh token
                    RefreshTokenUsage = TokenUsage.ReUse, //Could be one bcs we will get this everytime. OneTimeOnly
                    RefreshTokenExpiration = TokenExpiration.Absolute,
                    AbsoluteRefreshTokenLifetime = (int)(DateTime.Now.AddDays(60) - DateTime.Now).TotalSeconds,//SlidingRefreshTokenLifetime refresh -add time-
                    RequireConsent = false //Permissions view
                },
                new Client()
                {
                    ClientId ="Client2-Mvc",
                    RequirePkce=false,
                    ClientName= "Client2  MvcApp",
                    ClientSecrets =new[]{new Secret("secret".Sha256())},
                    AllowedGrantTypes= GrantTypes.Hybrid,
                    RedirectUris = new List<string>{ "https://localhost:5005/signin-oidc" }, //Protocol
                    PostLogoutRedirectUris = new List<string>{"https://localhost:5005/signout-callback-oidc"}, //Protocol - same in AuthServer
                    AllowedScopes = {IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile,"api1.read","api2.read",
                        IdentityServerConstants.StandardScopes.OfflineAccess,"CountryAndCity","Roles" },
                    AccessTokenLifetime = 2*60*60,//DateTime.Now.AddHours(2).Second,
                    AllowOfflineAccess = true,//Refresh token
                    RefreshTokenUsage = TokenUsage.ReUse, //Could be one bcs we will get this everytime. OneTimeOnly
                    RefreshTokenExpiration = TokenExpiration.Absolute,
                    AbsoluteRefreshTokenLifetime = (int)(DateTime.Now.AddDays(60) - DateTime.Now).TotalSeconds,//SlidingRefreshTokenLifetime refresh -add time-
                    RequireConsent = false //Permissions view
                },
                new Client()
                {
                    ClientId ="js-client",
                    RequireClientSecret = false, //bcs its js app
                    AllowedGrantTypes = GrantTypes.Code,
                    ClientName = "Angular Client",
                    AllowedScopes = {IdentityServerConstants.StandardScopes.Email, IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile,"api1.read",
                        IdentityServerConstants.StandardScopes.OfflineAccess,"CountryAndCity","Roles" },
                    RedirectUris={"http://localhost:4200/callback"},
                    AllowedCorsOrigins={"http://localhost:4200"},
                    PostLogoutRedirectUris={ "http://localhost:4200"}
                    
                },
                //DB
                new Client()
                {
                    ClientId ="Client1-ResourceOwner-Mvc",                  
                    ClientName = "Client1 App MvcApp",
                    ClientSecrets =new[]{new Secret("secret".Sha256())},
                    AllowedGrantTypes= GrantTypes.ResourceOwnerPassword,      
                    AllowedScopes = {IdentityServerConstants.StandardScopes.Email, IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile,"api1.read",
                        IdentityServerConstants.StandardScopes.OfflineAccess,"CountryAndCity","Roles" },
                    AccessTokenLifetime = 2*60*60,//DateTime.Now.AddHours(2).Second,
                    AllowOfflineAccess = true,//Refresh token
                    RefreshTokenUsage = TokenUsage.ReUse, //Could be one bcs we will get this everytime. OneTimeOnly
                    RefreshTokenExpiration = TokenExpiration.Absolute,
                    AbsoluteRefreshTokenLifetime = (int)(DateTime.Now.AddDays(60) - DateTime.Now).TotalSeconds//SlidingRefreshTokenLifetime refresh -add time-
                    
                },
            };
        }

    }
}
