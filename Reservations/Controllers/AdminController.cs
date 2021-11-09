using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Reservations.Classes;
using Reservations.Classes.Application.Home;
using Reservations.Classes.Utils;
using Reservations.Models;
using Reservations.ViewModels.Admin;
using Reservations.ViewModels.Reservation;

namespace Reservations.Controllers
{
    [AuthorizeUser(Roles = new UserRole[] { UserRole.HELP_DESK,
                                            UserRole.POWER_USER,
                                            UserRole.REGULAR_USER
                                           })]

    public class AdminController : Controller
    {
        [HttpGet]
        public ActionResult ListReservations()
        {
            DbManager db = new DbManager();
            ReservationVM r = new ReservationVM()
            {
                ReservationInfo = new Rsvn()
            };
            UserInfo info = (UserInfo)Session["UserInfo"];
            ApplicationHome app = new ApplicationHome();

            r.ReservationInfo.PeriodList = db.GetPeriodSelectList();
            r.ReservationQuery = app.GetAllReservationsDescription();

            return View(r);
        }

        [HttpPost]
        public ActionResult ListReservations(ReservationVM r, string button)
        {
            DbManager db = new DbManager();
            ApplicationHome app = new ApplicationHome();

            if(ModelState.IsValid)
            {
                if (button == "Търси")
                {
                    r.ReservationQuery = app.GetAllReservationsFilterDescription(r.FilterAdmin.AccomodationDateFrom, r.FilterAdmin.AccomodationDateTo,
                                                        r.FilterAdmin.LeaveDateFrom, r.FilterAdmin.LeaveDateTo,
                                                        r.FilterAdmin.StatusRsvn, r.FilterAdmin.ReservationMaker,
                                                        r.FilterAdmin.ReservationID);
                }
                else
                {
                    r.ReservationQuery = app.GetAllReservationsDescription();
                }

                return View(r);
            }

            return View(r);
        }

        [HttpGet]
        public ActionResult ReservationDetails(string id)
        {
            DbManager db = new DbManager();
            UserInfo info = (UserInfo)Session["UserInfo"];
            ReservationVM r = new ReservationVM()
            {
                ReservationInfo = db.GetRsvnData(id),
                GuestList = db.GetAllGuestsFromReservationId(id),
                RoomList = db.GetAllRooms(id)
            };
            
            r.ReservationInfo.RsvnStatusDescription = Tools.GetRsvnStatusEnumDescription((char)r.ReservationInfo.RsvnStatus);

            for (int i = 0; i < r.GuestList.Count; i++)
            {
                r.GuestList[i].PersonTypeDescription = Tools.GetPersonTypeEnumDescription((char)r.GuestList[i].PersonType);
                r.GuestList[i].PersonAgeDescription = Tools.GetPersonAgeEnumDescription((char)r.GuestList[i].PersonAge);
                r.GuestList[i].PersonGenderDescription = Tools.GetGenderEnumDescription((char)r.GuestList[i].Gender);
            }

            for (int i = 0; i < r.RoomList.Count; i++)
            {
                r.RoomList[i].RoomTypeDescription = Tools.GetRoomRoomTypeDescription(r.RoomList[i].RoomType);
                r.RoomList[i].RoomViewDescription = Tools.GetRoomRoomViewDescription(r.RoomList[i].RoomView);
                r.RoomList[i].RoomFloorDescription = Tools.GetRoomRoomFloorDescription(r.RoomList[i].RoomFloor);

            }

            return View("ReservationDetails", r);
        }

        [HttpPost]
        public ActionResult ReservationDetails(ReservationVM reservation, string button)
        {
            UserInfo info = (UserInfo)Session["UserInfo"];
            DbManager db = new DbManager();

            if (button == "Редакция")
            {
                return RedirectToAction("Edit", "Admin", new { id = reservation.ReservationInfo.ReservationID });//, /*reservation.ReservationInfo.ReservationID*/new { id = reservation.ReservationInfo.ReservationID });
            }
            else if(button == "Към резервации")
            {
                return RedirectToAction("ListReservations");
            }
            else if(button == "Промени статус")
            {
                if(ModelState.IsValid)
                {
                    db.UpdateReservationStatus(reservation.ReservationInfo, info);
                    return RedirectToAction("ListReservations");
                }
                return View(reservation);
            }

            return View(reservation);
            
        }


        [HttpGet]
        public ActionResult Edit(string id)
        {
            UserInfo info = (UserInfo)Session["UserInfo"];
            DbManager db = new DbManager();
            ReservationVM r = new ReservationVM()
            {
                ReservationInfo = db.GetRsvnData(id),
                GuestList = db.GetAllGuestsFromReservationId(id),
                RoomList = db.GetAllRooms(id)
            };

            r.ReservationInfo.PeriodList = db.GetPeriodSelectList().ToList(); // Filling Period List with data

            foreach (var room in r.RoomList)
            {
                room.RoomTypeList = db.GetRoomTypeSelectList();
                room.RoomViewList = db.GetRoomViewSelectList();
                room.RoomFloorList = db.GetRoomLevelSelectList();
                room.RoomIDList = db.GetAllRoomIDSelectList();
            }

            /* Filling Room ID List With only the rooms the Guests are staying */
            List<SelectListItem> RoomIDGuest = new List<SelectListItem>();
            for (int i = 0; i < r.GuestList.Count; i++)
            {
                RoomIDGuest.Add(new SelectListItem() { Text = r.GuestList[i].GuestRoomID.ToString(), Value = r.GuestList[i].GuestRoomID.ToString(), Selected = false });
            }
            for (int i = 0; i < r.GuestList.Count; i++)
            {
                r.GuestList[i].GuestRoomIDList = RoomIDGuest.ToList();
                r.GuestList[i].GuestRoomIDList = r.GuestList[i].GuestRoomIDList.GroupBy(x => x.Text).Select(x => x.First()).ToList();
            }         

            return View(r);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Edit(ReservationVM reservation, string button)
        {
            UserInfo info = (UserInfo)Session["UserInfo"];
            DbManager db = new DbManager();

            if (button == "Запази промяни")
            {
                if(ModelState.IsValid)
                {
                    // code for update.... UPDATE METHOD NEEDED
                    reservation.ReservationInfo.PeriodList = db.GetPeriodSelectList().ToList();
                    db.UpdateAll(reservation.ReservationInfo, reservation.RoomList, reservation.GuestList, info);
                    return RedirectToAction("ListReservations");
                }
                
                return View(reservation); // Error view needed, trybva da se doraboti
            }
            else if (button == "Отказ")
            {
                return RedirectToAction("ListReservations");
            }

            return View(reservation);

        }

        [HttpGet]
        public ActionResult ReservationQueries()
        {
            DbManager db = new DbManager();
            ApplicationHome app = new ApplicationHome();
            QueriesVM querieVM = new QueriesVM()
            {
                QueriesDataQuery2 = app.GetQuery2Data(),
                QueriesDataQuery3 = app.GetQuery3Data(),
                QueriesDataQuery1 = app.GetQuery1Data()
            };
            return View(querieVM);
        }

        [HttpPost]
        public ActionResult ReservationQueries(string button)
        {
            UserInfo userInfo = (UserInfo)Session["UserInfo"];
            
            if (button == "Обща информация - Excel")
            {
                ExcelManager querie1 = new ExcelManager();
                MemoryStream stream = querie1.ExportQuery1ToExcel();
                string fileName = string.Format("{0:Обща информация - yyyyMMddHHmmss}_report.xlsx", DateTime.Now);
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                stream.Position = 0;

                return File(stream, contentType, fileName);
            }
            else if(button == "Статус стаи - Excel")
            {
                ExcelManager querie2 = new ExcelManager();
                MemoryStream stream = querie2.ExportQuery2ToExcel();
                string fileName = string.Format("{0:Статус стаи - yyyyMMddHHmmss}_report.xlsx", DateTime.Now);
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                stream.Position = 0;

                return File(stream, contentType, fileName);
            }
            else if(button == "Освобождаващи се стаи - Excel")
            {
                ExcelManager querie3 = new ExcelManager();
                MemoryStream stream = querie3.ExportQuery3ToExcel();
                string fileName = string.Format("{0:Освобождаващи се стаи - yyyyMMddHHmmss}_report.xlsx", DateTime.Now);
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                stream.Position = 0;

                return File(stream, contentType, fileName);
            }

            return View();
        }
    }
}