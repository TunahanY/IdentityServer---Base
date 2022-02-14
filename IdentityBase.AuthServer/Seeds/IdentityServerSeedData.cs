using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityBased.AuthServer.Seeds
{
    public static class IdentityServerSeedData
    {
        public static void Seed(ConfigurationDbContext context)
        {
            //context.Database.EnsureCreated()
            if(!context.Clients.Any())
            {
                foreach (var client in Config.GetClients())
                {
                    context.Clients.Add(client.ToEntity());
                }
            }

            if(!context.ApiResources.Any())
            {
                foreach(var apiResource in Config.GetApiResources())
                {
                    context.ApiResources.Add(apiResource.ToEntity());
                }
            } 

            if(!context.ApiScopes.Any())
            {
                Config.GetApiScopes().ToList().ForEach(apiScopes =>
                {
                    context.ApiScopes.Add(apiScopes.ToEntity());
                });
            }

            if(!context.IdentityResources.Any())
            {
                Config.GetIdentityResources().ToList().ForEach(identityResources =>
                {
                    context.IdentityResources.Add(identityResources.ToEntity());
                });
            }

            context.SaveChanges();

        }
    }
}
