using IdentityBased.AuthServer.Repository;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityBased.AuthServer.Services
{
    public class CustomProfileService : IProfileService
    {
        private readonly ICustomUserRepository _customRepository;
        public CustomProfileService(ICustomUserRepository customRepository)
        {
            _customRepository = customRepository;
        }
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subId = context.Subject.GetSubjectId();
            var user = await _customRepository.FindById(int.Parse(subId));
            //Which claims do I need?
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email), // "email" we can reach user infos without creating transaction with CLAIMS.
                new Claim("name", user.UserName),
                new Claim("username", user.UserName), //ProductsController -> UserName
                new Claim("city", user.City),
            };

            if(user.Id == 1)
            {
                claims.Add(new Claim("role", "admin"));
            }
            else
            {
                claims.Add(new Claim("role", "customer"));
            }

            context.AddRequestedClaims(claims);

            context.IssuedClaims = claims;//If you want to see information in JWT - 
            //Sample Token with claims, informations..
            //eyJhbGciOiJSUzI1NiIsImtpZCI6IkJERjhEODRDMjNERUI4MUYxRkEwRTAwQzJBMTU0RTA1IiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE2NDIzNjg0MTEsImV4cCI6MTY0MjM3NTYxMSwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NTAwMSIsImF1ZCI6InJlc291cmNlX2FwaTEiLCJjbGllbnRfaWQiOiJDbGllbnQxLU12YyIsInN1YiI6IjEiLCJhdXRoX3RpbWUiOjE2NDIzNjg0MTEsImlkcCI6ImxvY2FsIiwiZW1haWwiOiJtdHVuYWhhbnlvbGxhckBvdXRsb29rLmNvbSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJ0dW5haGFueW9sbGFyIiwidXNlcm5hbWUiOiJ0dW5haGFueW9sbGFyIiwiY2l0eSI6IkFua2FyYSIsInJvbGUiOiJhZG1pbiIsImp0aSI6Ijk2NUZGRTg0RjI0OTNBNzQ1QUU1NERDMUQxRjYxMURGIiwic2lkIjoiRDAwM0I1RThBMjJBMzk1N0NFQ0FGMzRENzMzQkUxNkEiLCJpYXQiOjE2NDIzNjg0MTEsInNjb3BlIjpbIm9wZW5pZCIsInByb2ZpbGUiLCJhcGkxLnJlYWQiLCJDb3VudHJ5QW5kQ2l0eSIsIlJvbGVzIiwib2ZmbGluZV9hY2Nlc3MiXSwiYW1yIjpbInB3ZCJdfQ.THaLjIWwCpHKfYd8dbKEW9OIPwIiVjF4e-GhQdICJGy6_aRrxE-uOPuCrDn5f2TfClcNk4_71GvR6Ew8puP-YMkDphVsJA71v9RTLWDZBr8rqUhtSqUTLXjrzwkdoqZxQjlvnMQ2-VJ71uxXaYPjtCnNuQ3Zo6ac-DYE-pmm6GW5hvVvmYGr-QAT_STnztzZzTnqai9xyOa01y-YJuLRf45mVKZf53l9umwSj6dDWHEjkADi81clvZVWU51UGgcgfgXKYUWNYCFUW-Fx6ViJcmkoScTAkHPQNYh57luB5MYtXhql-0usdD48_CyncJOh1ottmXj6oME3fNZCSJhq7g
            //
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var userId = context.Subject.GetSubjectId();
            var user = await _customRepository.FindById(int.Parse(userId));
            context.IsActive = user != null ? true : false;
        }
    }
}
