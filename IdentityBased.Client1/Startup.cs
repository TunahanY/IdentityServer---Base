using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityBased.Client1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(opts =>
            {
                opts.DefaultScheme = "Cookies"; //İK ve BT (bayi müşteri..) gibi 2 tane tanımlanabilir.
                opts.DefaultChallengeScheme = "oidc";

                //Client talking with AuthServer
            }).AddCookie("Cookies").AddOpenIdConnect("oidc", opts =>
            {
                opts.SignInScheme = "Cookies";
                opts.Authority = "https://localhost:5001"; //Who is in charge?
                opts.ClientId = "Client1-Mvc";
                opts.ClientSecret = "secret";   
                opts.ResponseType = "code id_token";
                opts.GetClaimsFromUserInfoEndpoint = true; //We can get users info via coookkkiiiee {given_name/last_name etc.}
                opts.SaveTokens = true;
                opts.Scope.Add("api1.read"); //our permissions
                opts.Scope.Add("offline_access"); //RefreshToken
                opts.Scope.Add("CountryAndCity");
                opts.Scope.Add("Roles");
                opts.ClaimActions.MapUniqueJsonKey("country", "country");//country in token - mapped
                opts.ClaimActions.MapUniqueJsonKey("city", "city"); //city in token - mapped
                opts.ClaimActions.MapUniqueJsonKey("role", "role");

                opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    RoleClaimType = "role"
                };

            });
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
