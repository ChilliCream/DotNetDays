using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using WorkshopChatServer.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace WorkshopChatServer.Types.User
{
    [ExtendObjectType("Mutation")]
    public class UserMutation
    {
        public const string ClaimName = "UserName";

        public Task<User> CreateUser(
            [Service] IUserRepository userRepository,
            string name)
        {
            return userRepository.CreateUser(name);
        }

        public async Task<User> UserLogin(
            [Service] IHttpContextAccessor httpContextAccessor,
            string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(
                    ClaimName,
                    userName)
            };
            
            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );
            
            var authProperties = new AuthenticationProperties {
                AllowRefresh = false, ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(900)
            };
            
            await httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );
            
            return new User(){ Name = userName, LastSeen = DateTime.Now};
        }
    }
}