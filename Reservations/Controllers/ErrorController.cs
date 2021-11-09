using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Reservations.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult RoomReservedAtSameTime()
        {
            return View();
        }

        [HttpGet]
        public ActionResult BasicException()
        {
            return View();
        }

        [HttpPost]
        public ActionResult BasicException(string button)
        {
            if(button == "Към търсачка")
            {
                return RedirectToAction("Index", "Reservation");
            }


            return View();
        }

        [HttpGet]
        public ActionResult IEException()
        {
            return View();
        }
    }
}