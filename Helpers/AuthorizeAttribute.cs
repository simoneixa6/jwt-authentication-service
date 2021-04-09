using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using jwt_authentication_service.Models;

namespace jwt_authentication_service.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = (User)context.HttpContext.Items["User"];
            if (user == null)
            {
                // L'utilisateur n'est pas connecté
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = 401 };
            }
        }
    }
}
