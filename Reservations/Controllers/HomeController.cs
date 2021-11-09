using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Reservations.Classes;
using Reservations.Classes.Application.Home;
using Reservations.Classes.Utils;

namespace Reservations.Controllers
{
    [AuthorizeUser(Roles = new UserRole[] { UserRole.HELP_DESK,
                                            UserRole.POWER_USER,
                                            UserRole.REGULAR_USER
                                           })]

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            UserInfo info = (UserInfo)Session["UserInfo"];

            return View();
        }

        public ActionResult About()
        {
            ApplicationHome applicationHome = new ApplicationHome();

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}