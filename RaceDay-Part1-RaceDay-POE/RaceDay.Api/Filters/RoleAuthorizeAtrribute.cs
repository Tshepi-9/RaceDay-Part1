using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RaceDay.Api.Filters
{
    public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _allowedRoles;

    public RoleAuthorizeAttribute(params string[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userId = context.HttpContext.Session.GetInt32("UserId");
            var userRole = context.HttpContext.Session.GetString("Role");

            if (userId == null || string.IsNullOrEmpty(userRole))
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    message = "You must log in first."
                });

                return;
            }

            if (!_allowedRoles.Contains(userRole, StringComparer.OrdinalIgnoreCase))
            {
                context.Result = new ObjectResult(new
                {
                    message = "You do not have permission to perform this action."
                })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
        }
    }

}
