using Microsoft.AspNetCore.Authorization;
using Zavrsni.Model;

namespace Zavrsni.Web.Auth
{
    public class UserTypeRequirement : IAuthorizationRequirement
    {
        public UserType RequiredUserType { get; }

        public UserTypeRequirement(UserType requiredUserType)
        {
            RequiredUserType = requiredUserType;
        }
    }
}
