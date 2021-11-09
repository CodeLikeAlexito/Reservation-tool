using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Reservations.Classes.Utils;
using UCB.SecurityManager;

namespace Reservations
{
    public class MvcApplication : System.Web.HttpApplication
    {
        UCB.SecurityManager.AccessManager accessManager;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            log4net.Config.XmlConfigurator.Configure();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            String SecurityDirectory = ConfigurationManager.AppSettings["SecurityDirectory"].ToString();

            //string userName = User.Identity.Name;
            string userName = "BB049915";

            //  userName = "BB046393";

            UCB.SecurityManager.SecurityManager secManager = new UCB.SecurityManager.SecurityManager(SecurityDirectory, userName);

            accessManager = secManager.getAccessManager();

            Session["UserInfo"] = UserRole.POWER_USER;//GetUserInfo(accessManager);
        }

        private UserInfo GetUserInfo(AccessManager accessManager)
        {
            UserInfo info = new UserInfo();

            info.accessManager = accessManager;

            info.Role = GetUserRole(accessManager);

            return info;
        }

        private UserRole GetUserRole(AccessManager accessManager)
        {

            if (accessManager.AccessRoles.Contains("POWER_USER"))
                return UserRole.POWER_USER;

            if (accessManager.AccessRoles.Contains("REGULAR_USER"))
                return UserRole.REGULAR_USER;

            if (accessManager.AccessRoles.Contains("HELP_DESK"))
                return UserRole.HELP_DESK;

            /*if (accessManager.AccessRoles.Contains("CORP_EDITOR"))
                return UserRole.CORP_EDITOR; */

            return UserRole.NONE;
        }
    }
}
