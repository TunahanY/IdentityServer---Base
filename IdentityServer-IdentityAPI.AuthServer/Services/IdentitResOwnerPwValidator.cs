using IdentityModel;
using IdentityServer4.Validation;
using IdentityServerIdentityAPI.AuthServer.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerIdentityAPI.AuthServer.Services
{
    public class IdentitResOwnerPwValidator : IResourceOwnerPasswordValidator
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentitResOwnerPwValidator(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            //UserName to Email CRASSHED
            var existUser = await _userManager.FindByEmailAsync(context.UserName); if (existUser == null) return;

            var checkPassword = await _userManager.CheckPasswordAsync(existUser, context.Password); if (checkPassword == false) return;

            context.Result = new GrantValidationResult(existUser.Id.ToString(), OidcConstants.AuthenticationMethods.Password);

        }
    }
}
