using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RaceDay.Api.Filters
{
    public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _requiredRole;

    public RoleAuthorizeAttribute(string requiredRole)
        {
            _requiredRole = requiredRole;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userId = context.HttpContext.Session.GetInt32("UserId");
            var userRole = context.HttpContext.Session.GetString("Role");

            // Check whether the user is logged in.
            if (userId == null || string.IsNullOrEmpty(userRole))
            {
                context.Result = new UnauthorizedObjectResult(
                    new { message = "Please log in first." });

                return;
            }

            // Check whether the user has the required role.
            if (!string.Equals(userRole, _requiredRole,
                StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new ObjectResult(
                    new { message = "You do not have permission to perform this action." })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
        }
    }

}
