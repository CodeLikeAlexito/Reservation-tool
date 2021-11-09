
using Reservations.Classes.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UCB.SecurityManager;

namespace Reservations.Classes
{
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        // Custom property
        public UserRole[] Roles;

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            UserInfo info = ((UserInfo)httpContext.Session["UserInfo"]);

            foreach (string role in info.accessManager.AccessRoles)
            {
                if (Roles.Contains((UserRole)Enum.Parse(typeof(UserRole), role)))
                    return true;
            }

            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            string path = string.Format("-{0}-{1}", filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, filterContext.ActionDescriptor.ActionName);

            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "AccessDenied", id = path }));
        }
    }
}