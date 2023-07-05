using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Twilio.Rest.Chat.V2.Service.User;

namespace Atlice.Domain.Abstract
{
    public class MobileAuthorizationHandler : AuthorizationHandler<IMobileAuthorizationHandler>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;

        public MobileAuthorizationHandler(IHttpContextAccessor httpContextAccessor, IConfiguration config)
        {
            _httpContextAccessor = httpContextAccessor;
            _config = config;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IMobileAuthorizationHandler requirement)
        {
            var jwt = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);

            if (string.IsNullOrEmpty(jwt))
            {
                return Task.CompletedTask;
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters();
            SecurityToken validatedToken;
            IPrincipal principal = tokenHandler.ValidateToken(jwt, validationParameters, out validatedToken);

            if (principal.Identity.IsAuthenticated)
            {
                context.Succeed(requirement);

                return Task.CompletedTask;
            }
            
            context.Fail();
            return Task.CompletedTask;
        }
        private TokenValidationParameters GetValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateLifetime = false, // Because there is no expiration in the generated token
                ValidateAudience = false, // Because there is no audiance in the generated token
                ValidateIssuer = false,   // Because there is no issuer in the generated token
                ValidIssuer = "Sample",
                ValidAudience = "Sample",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])) // The same key as the one that generate the token
            };
        }
    }
    public class IMobileAuthorizationHandler : IAuthorizationRequirement
    {
        public IMobileAuthorizationHandler()
        {
            MaximumOfficeNumber = "a@g.com";
        }

        public string MaximumOfficeNumber { get; private set; }
    }
}
