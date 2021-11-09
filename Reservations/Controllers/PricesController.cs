using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Reservations.Classes;
using Reservations.Classes.Application.Home;
using Reservations.Classes.Utils;
using Reservations.Models;
using Reservations.ViewModels.Prices;

namespace Reservations.Controllers
{
    [AuthorizeUser(Roles = new UserRole[] { UserRole.HELP_DESK,
                                            UserRole.POWER_USER,
                                            UserRole.REGULAR_USER
                                           })]
    public class PricesController : Controller
    {

        [HttpGet]
        public ActionResult Prices()
        {
            UserInfo info = (UserInfo)Session["UserInfo"];

            ApplicationHome app = new ApplicationHome();
            var res = app.GetAllPrices();

            return View(res);
        }

        [HttpGet]
        public ActionResult EditPrices(int id)
        {
            UserInfo info = (UserInfo)Session["UserInfo"];
            DbManager db = new DbManager();

            PricesVM viewModel = new PricesVM()
            {
                PricesModel = db.GetPricesByID(id),
                RoomViewList = db.GetDistinctRoomDesc(),
                PersonTypeList = db.GetDistinctPersonTypePricesTable(),
                SeasonList = db.GetDistinctSeason(),
                UCBList = db.GetDistinctUCBType()
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditPrices(PricesVM vm, string button)
        {
            UserInfo info = (UserInfo)Session["UserInfo"];
            DbManager db = new DbManager();

            if(button == "Запази промяни")
            {
                if(ModelState.IsValid)
                {
                    db.UpdatePricesTable(vm.PricesModel);
                    return RedirectToAction("Prices");
                }

                PricesVM viewModel = new PricesVM()
                {
                    PricesModel = db.GetPricesByID(vm.PricesModel.ID),
                    RoomViewList = db.GetDistinctRoomDesc(),
                    PersonTypeList = db.GetDistinctPersonTypePricesTable(),
                    SeasonList = db.GetDistinctSeason(),
                    UCBList = db.GetDistinctUCBType()
                };

                return View(viewModel);

            }
            else if(button == "Отказ")
            {
                return RedirectToAction("Prices");
            }

            return View();
        }
    }
}