using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Zavrsni.Model;

namespace Zavrsni.Web.Auth
{
    public class UserTypeHandler : AuthorizationHandler<UserTypeRequirement>
    {
        private readonly UserManager<AppUser> _userManager;

        public UserTypeHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserTypeRequirement requirement)
        {
            
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(context.User);
                if (user != null && user.UserType == requirement.RequiredUserType)
                {
                    context.Succeed(requirement);
                }
            }
            await Task.CompletedTask;
        }
    }
}
