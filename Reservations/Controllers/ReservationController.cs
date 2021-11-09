using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Oracle.ManagedDataAccess.Client;
using Reservations.Classes;
using Reservations.Classes.Application.Home;
using Reservations.Classes.Utils;
using Reservations.Models;
using Reservations.ViewModels.Reservation;

namespace Reservations.Controllers
{
    public class ReservationController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            Log.Info("GET/INDEX: Index page of reservation started ...");
            DbManager db = new DbManager();

            Log.Info("GET/INDEX: Creating new instance of Rsvn");
            ReservationVM r = new ReservationVM()
            {
                ReservationInfo = new Rsvn()
            };

            Log.Info("GET/INDEX: Setting period list to a values from DB");
            r.ReservationInfo.PeriodList = db.GetPeriodSelectList();

            Log.Info("GET/INDEX: Returning Index view");
            return View(r);
        }

        [HttpPost]
        public ActionResult Index(ReservationVM r)
        {
            Log.Info("POST/INDEX: Post method / INDEX view..");
            DbManager db = new DbManager();

            Log.Info("POST/INDEX: Checkig if ModelState is NOT valid..");
            if (!ModelState.IsValid)
            {
                Log.Warn("POST/INDEX: Model state is not valid. Returning Index view..");
                r.ReservationInfo.PeriodList = db.GetPeriodSelectList().ToList();
                return View(r);
            }

            Log.Info("POST/INDEX: Model State is valid. Everything ok. Going to SearchResult View");
            Session["indexDataRsvn"] = (ReservationVM)r;
            return RedirectToAction("SearchResult");
        }

        [HttpGet]
        public ActionResult SearchResult(ReservationVM r)
        {
            try
            {
                if (Session["indexDataRsvn"] != null)
                {
                    Log.Info("GET/SEARCHRESULT: View Model is not empty.");
                    var data = Session["indexDataRsvn"];
                    r = (ReservationVM)data;
                }
                else
                {
                    try
                    {
                        Log.Info("GET/SEARCHRESULT: View Model is empty.");
                        Log.Error("GET/SEARCHRESULT: Throwing NullReferenceException.");
                        throw new NullReferenceException();
                        
                    }
                    catch (NullReferenceException ex)
                    {
                        Log.Error("GET/SEARCHRESULT: Exception occured {0}", ex);
                        return RedirectToAction("BasicException", "Error");
                    }
                }

                DbManager db = new DbManager();
                ApplicationHome app = new ApplicationHome();
                UserInfo info = (UserInfo)Session["UserInfo"];

                int? maxPersons = r.ReservationInfo.Adults + r.ReservationInfo.Kids;
                if (maxPersons is null)
                {
                    maxPersons = 0;
                }

                r.ReservationInfo.PeriodList = db.GetPeriodSelectList();
                r.AvailableRooms = app.GetGroupAvailableRoomsDescription(r);

                return View(r);
            }
            catch(OracleException oEx)
            {
                string message = String.Format("Exception type: {0} occured. Message: {1}", oEx.GetType().FullName, oEx.Message);
                Log.Error("GET/SEARCHRESULT: {0}", oEx);
                return RedirectToAction("BasicException", "Error");
            }
            catch(Exception ex)
            {
                string message = String.Format("Exception type: {0} occured. Message: {1}", ex.GetType().ToString(), ex.Message);
                Log.Error("GET/SEARCHRESULT: {0}", ex);
                return RedirectToAction("BasicException", "Error");
            }
            
        }

        [HttpPost]
        public ActionResult SearchResult(ReservationVM r, string button, FormCollection formM)
        {
            DbManager db = new DbManager();
            ApplicationHome app = new ApplicationHome();
            UserInfo info = (UserInfo)Session["UserInfo"];

            int row_id = -1;
            var selectedID = formM.GetValues("button"); // Value of Clicked Button From List of Rooms       

            /* If button is Търси Show different results on Screen */
            if (button == "Ново търсене")
            {
                if (ModelState.IsValid)
                {
                    r.ReservationInfo.PeriodList = db.GetPeriodSelectList().ToList();
                    r.AvailableRooms = app.GetGroupAvailableRoomsDescription(r);
                    return View(r);
                }

                r.ReservationInfo.PeriodList = db.GetPeriodSelectList().ToList();
                r.AvailableRooms = app.GetGroupAvailableRoomsDescription(r);
                return View(r); // Error View .. trqbva da se promeni 
            }
            /* Checking if some of the Grid Room Options is Clicked */
            else if (button == selectedID[0])
            {
                if (ModelState.IsValid)
                {
                    /*  Pass List Of Available Rooms to Reservation View 
                     *  We need it for further Logic 
                    */
                    r.AvailableRooms = app.GetGroupAvailableRoomsDescription(r);
                    row_id = Int32.Parse(selectedID[0]); // Converting from string to Int
                    Session["reservationVM"] = (ReservationVM)r;
                    Session["rowId"] = row_id;
                    return RedirectToAction("Reservation");
                }

                return View(r); // Trqbva da vyrne Error view, trqbva da se doraboti

            }

            return View(r);

        }

        [HttpGet]
        public ActionResult Reservation(ReservationVM r)
        {
            DbManager db = new DbManager();
            UserInfo info = (UserInfo)Session["UserInfo"];
            AvailableRooms tempRoom;// = new AvailableRooms();
            int? roomId = null;
            //int firstRoom = 0;
            int? maxPersons = null;


            try
            {
                if (Request.UrlReferrer.AbsolutePath == "/Reservation/Details")
                {

                    var dataForCorr = Session["DataForCorrection"];
                    r = (ReservationVM)dataForCorr;

                    var roomData = Session["rowId"]; // Selected First Room Data

                    roomId = (int?)roomData;
                    tempRoom = new AvailableRooms();

                    r.ReservationInfo.RsvnMaker = info.accessManager.BBUser;
                    r.ReservationInfo.Nights = (((DateTime)r.ReservationInfo.EndDATE - (DateTime)r.ReservationInfo.BegDATE).TotalDays) + 1;
                    maxPersons = r.ReservationInfo.Adults + r.ReservationInfo.Kids;

                    r.ReservationInfo.PeriodList = db.GetPeriodSelectList();
                    r.ReservationInfo.BegDATEDescription = Tools.ConvertDateTimeNullableToString(r.ReservationInfo.BegDATE);
                    r.ReservationInfo.EndDATEDescription = Tools.ConvertDateTimeNullableToString(r.ReservationInfo.EndDATE);

                    for (int i = 0; i < r.ReservationInfo.Rooms; i++)
                    {
                        r.RoomList[i].RoomTypeList = db.GetRoomTypeSelectList();                       
                        r.RoomList[i].RoomViewList = db.GetRoomViewSelectList();
                        r.RoomList[i].RoomFloorList = db.GetRoomLevelSelectList();
                        r.RoomList[i].RoomIDList = db.GetAllRoomIDSelectList();
                    }

                    for (int i = 0; i < maxPersons; i++)
                    {
                        r.GuestList[i].GuestRoomIDList = db.GetAllRoomIDSelectList();
                    }

                    return View(r);
                }
                else
                {
                    var data = Session["reservationVM"]; // Reservation Data
                    var roomData = Session["rowId"]; // Selected First Room Data

                    r = (ReservationVM)data;
                    roomId = (int?)roomData;
                    tempRoom = new AvailableRooms();

                    r.ReservationInfo.RsvnMaker = info.accessManager.BBUser;
                    r.ReservationInfo.Nights = (((DateTime)r.ReservationInfo.EndDATE - (DateTime)r.ReservationInfo.BegDATE).TotalDays) + 1;
                    maxPersons = r.ReservationInfo.Adults + r.ReservationInfo.Kids;

                    /* Get Room Type, Room View For the First Room From SearchResult View selected option */
                    foreach (var selectedRoom in r.AvailableRooms)
                    {
                        if (roomId == selectedRoom.Id)
                        {
                            tempRoom = selectedRoom;
                            break;
                        }
                    }

                    r.ReservationInfo.PeriodList = db.GetPeriodSelectList().ToList();
                    r.ReservationInfo.BegDATEDescription = Tools.ConvertDateTimeNullableToString(r.ReservationInfo.BegDATE);
                    r.ReservationInfo.EndDATEDescription = Tools.ConvertDateTimeNullableToString(r.ReservationInfo.EndDATE);
                    r.RoomList = new List<Room>();
                    r.GuestList = new List<Guest>();

                    for (int i = 0; i < r.ReservationInfo.Rooms; i++)
                    {
                        r.RoomList.Add(new Room());

                        if (i == 0)
                        {
                            /* Creating Temp Lists When For The First Room */
                            List<SelectListItem> TempRoomTypeSelectList = new List<SelectListItem>();
                            List<SelectListItem> TempRoomViewSelectList = new List<SelectListItem>();
                            TempRoomTypeSelectList.Add(new SelectListItem() { Text = tempRoom.RoomTypeDescription, Value = tempRoom.RoomType.ToString(), Selected = true });
                            TempRoomViewSelectList.Add(new SelectListItem() { Text = tempRoom.RoomViewDescription, Value = tempRoom.RoomView.ToString(), Selected = true });

                            /* Setting The Model Lists From The Temp Lists */
                            r.RoomList[i].RoomTypeList = TempRoomTypeSelectList.ToList();
                            r.RoomList[i].RoomViewList = TempRoomViewSelectList.ToList();

                            /* Getting Room Floor List Values for First Room */
                            r.RoomList[i].RoomFloorList = db.GetRoomLevelSelectList();
                            /* Getting Room Id List Values for First Room*/
                            r.RoomList[i].RoomIDList = db.GetAllRoomIDSelectList();

                            //flag first room
                            r.RoomList[i].IsMainRoom = 'Y';
                        }
                        else
                        {
                            /* List Values for the 2nd, 3th, Nth Room */
                            r.RoomList[i].RoomTypeList = db.GetRoomTypeSelectList();
                            r.RoomList[i].RoomViewList = db.GetRoomViewSelectList();
                            r.RoomList[i].RoomFloorList = db.GetRoomLevelSelectList();
                            r.RoomList[i].RoomIDList = db.GetAllRoomIDSelectList();

                            // flag not first room
                            r.RoomList[i].IsMainRoom = 'N';
                        }
                    }

                    for (int i = 0; i < maxPersons; i++)
                    {
                        r.GuestList.Add(new Guest());

                        if (i < r.ReservationInfo.Adults)
                        {
                            r.GuestList[i].PersonAge = PersonAge.Adult;
                        }
                        else
                        {
                            r.GuestList[i].PersonAge = PersonAge.Kid;
                        }

                        if (i == 0)
                            r.GuestList[i].IsMainGuest = 'Y';
                        else
                            r.GuestList[i].IsMainGuest = 'N';

                        r.GuestList[i].GuestRoomIDList = db.GetAllRoomIDSelectList();
                    }

                    return View(r);
                }
            }
            catch(Exception e)
            {
                var data = Session["reservationVM"]; // Reservation Data
                var roomData = Session["rowId"]; // Selected First Room Data

                r = (ReservationVM)data;
                roomId = (int?)roomData;
                tempRoom = new AvailableRooms();

                r.ReservationInfo.RsvnMaker = info.accessManager.BBUser;
                r.ReservationInfo.Nights = (((DateTime)r.ReservationInfo.EndDATE - (DateTime)r.ReservationInfo.BegDATE).TotalDays) + 1;
                maxPersons = r.ReservationInfo.Adults + r.ReservationInfo.Kids;

                return View(r);
            }
        }

        [HttpPost]
        public ActionResult Reservation(ReservationVM r, string button)
        {
            ApplicationHome applicationHome = new ApplicationHome();
            DbManager db = new DbManager();

            if (button == "Към търсачка")
            {
                return RedirectToAction("Index");
            }
            else
            {
                Session["dataFromRsvnToDetails"] = (ReservationVM)r;
                if (ModelState.IsValid)
                {
                    r.ReservationInfo.TotalPrice = applicationHome.CalculateReservationPrice(r.ReservationInfo, r.RoomList, r.GuestList);
                    return RedirectToAction("Details");
                }
                else
                {
                    r.ReservationInfo.PeriodList = db.GetPeriodSelectList().ToList();
                    for (int i = 0; i < r.ReservationInfo.Rooms; i++)
                    {
                        r.RoomList[i].RoomFloorList = db.GetRoomLevelSelectList();
                        r.RoomList[i].RoomIDList = db.GetAllRoomIDSelectList();
                        r.RoomList[i].RoomTypeList = db.GetRoomTypeSelectList();
                        r.RoomList[i].RoomViewList = db.GetRoomViewSelectList();
                    }
                    int? maxPersons = r.ReservationInfo.Adults + r.ReservationInfo.Kids;
                    for (int i = 0; i < maxPersons; i++)
                    {
                        r.GuestList[i].GuestRoomIDList = db.GetAllRoomIDSelectList();
                    }
                    return View(r);
                }

            }

        }

        [HttpGet]
        public ActionResult Details(ReservationVM r)
        {
            AvailableRooms tempRoom;
            int? roomId = null;

            var data = Session["dataFromRsvnToDetails"];
            r = (ReservationVM)data;


            ApplicationHome applicationHome = new ApplicationHome();


            for (int i = 0; i < r.RoomList.Count; i++)
            {
                r.RoomList[i].RoomTypeDescription = Tools.GetRoomRoomTypeDescription(r.RoomList[i].RoomType);
                r.RoomList[i].RoomViewDescription = Tools.GetRoomRoomViewDescription(r.RoomList[i].RoomView);
                r.RoomList[i].RoomFloorDescription = Tools.GetRoomRoomFloorDescription(r.RoomList[i].RoomFloor);
            }

            for (int i = 0; i < r.GuestList.Count; i++)
            {
                r.GuestList[i].PersonTypeDescription = Tools.GetPersonTypeEnumDescription((char)r.GuestList[i].PersonType);
                r.GuestList[i].PersonAgeDescription = Tools.GetPersonAgeEnumDescription((char)r.GuestList[i].PersonAge);
                r.GuestList[i].PersonGenderDescription = Tools.GetGenderEnumDescription((char)r.GuestList[i].Gender);
                r.GuestList[i].BirthDateDescription = Tools.ConvertDateTimeNullableToString(r.GuestList[i].BirthDate);
            }

            return View(r);
        }


        [HttpPost]
        public ActionResult Details(ReservationVM r, string button)
        {
            var res = Session["dataFromRsvnToDetails"];
            r = (ReservationVM)res;
            DbManager db = new DbManager();
            UserInfo info = (UserInfo)Session["UserInfo"];
            Emailer email = new Emailer();
            string EmailTo = info.accessManager.UserEmail;

            if (ModelState.IsValid)
            {
                Log.Info("POST/DETAILS: Model State is valid. Trying to make the reservation");
                try
                {
                    r.ReservationInfo.RsvnDATE = DateTime.Now;
                    r.ReservationInfo.PaymentDATE = null;
                    r.ReservationInfo.CancellationDATE = null;
                    r.ReservationInfo.AccommodationDATE = r.ReservationInfo.BegDATE;
                    r.ReservationInfo.LeavingDATE = r.ReservationInfo.EndDATE;
                    r.ReservationInfo.RsvnStatus = StatusRsvn.Payment;
                    r.ReservationInfo.PaymentRefNo = null;
                    r.ReservationInfo.Comment = null;

                    Log.Info($"POST/DETAILS: Inside try.");
                    Log.Info($"POST/DETAILS: Reservaiton date: {r.ReservationInfo.RsvnDATE}, Reservation status: {r.ReservationInfo.RsvnStatus}.");

                    if (r.ReservationInfo.Period == null)
                    {
                        Log.Info($"POST/DETAILS: Period is null. Notification for Date to Date.");
                        email.SendMailForDateToDateRequests(r.ReservationInfo.BegDATE, r.ReservationInfo.EndDATE, info.accessManager.UserEmail, r.RoomList, r.ReservationInfo.TotalPrice);
                    }
                    else
                    {
                        // email method for period notif
                        Log.Info($"POST/DETAILS: Period is not null. Notification for Periods.");
                        email.SendMailForPeriodRequests(r.ReservationInfo.BegDATE, r.ReservationInfo.EndDATE, info.accessManager.UserEmail, r.RoomList, r.ReservationInfo.TotalPrice);
                    }

                    db.InsertAll(r.ReservationInfo, r.RoomList, r.GuestList);

                    return Json(new { success = true, responseText = "Success" }, JsonRequestBehavior.AllowGet);
                }
                catch (OracleException e)
                {
                    Log.Error($"OracleException. ErrorCode: {e.ErrorCode}, ErrorMessage: {e.Message}.");
                    string errorMessage = "Code: " + e.ErrorCode + "\n" +
                       "Message: " + e.Message;


                    // return error view
                    return Json(new { success = false, responseText = "Oracle fail" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    Log.Error($"Exception occured. Exception stack trace: {e.StackTrace}. Exception message: {e.Message}. ");
                    string errorMessage = "Code: "+ "\n" +
                       "Message: " + e.Message;

                    // return error view
                    return Json(new { success = false, responseText = e.Message }, JsonRequestBehavior.AllowGet);
                }


            }
            else if(button == "Направи корекция")
            {
                Session["DataForCorrection"] = (ReservationVM)r;

                return RedirectToAction("Reservation");
            }
            else
            {
                return View(r);
            }
        }

        public JsonResult IsCorrectEGN()
        {
            var guestList = Request.Form[Request.Form.AllKeys.First(p => p.Contains("Guest"))];
            
            return Json(CheckEGN(guestList), JsonRequestBehavior.AllowGet);
        }

        public bool CheckEGN(string EGN)
        {
            DbManager db = new DbManager();
            int result = db.CheckIfEGNISValid(EGN);
            bool isValidEgn = false;

            if (result == 1)
                isValidEgn = true;

            return isValidEgn;
        }

        public JsonResult CheckPersonAge()
        {
            try
            {
                var guestList = Request.Form[Request.Form.AllKeys.First(p => p.Contains("Guest"))];
                DateTime temp = Convert.ToDateTime(guestList);
            }
            catch(Exception e)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            
            return Json(true, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult GetBegEndDates(int? value)
        {
            DbManager db = new DbManager();
            List<PeriodsTableModel> periodsTable = new List<PeriodsTableModel>();
            periodsTable = db.GetStartEndDateOfPeriod(value);

            if (value != null)
            {
                Rsvn reservation = new Rsvn()
                {
                    BegDATE = (DateTime)periodsTable[0].BeginDate,
                    EndDATE = (DateTime)periodsTable[0].EndDate
                };
                return Json(new { Success = "true", Data = new { BegDATE = (DateTime)periodsTable[0].BeginDate, EndDATE = (DateTime)periodsTable[0].EndDate } });
            }
            return Json(new { Success = "false" });
        }

        [HttpGet]
        public JsonResult FetchRoomView(char? roomType)
        {
            DbManager db = new DbManager();
            ApplicationHome app = new ApplicationHome();

            var roomView = db.GetRoomViewSelectListFromType(roomType).ToList();

            return Json(roomView, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FetchRoomFloor(char? roomType, char? roomView)
        {
            DbManager db = new DbManager();

            var roomFloor = db.GetRoomFloorSelectListFromRoomTypeView(roomType, roomView).ToList();

            return Json(roomFloor, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FetchRoomID(string begDate, string endDate, char? roomType, char? roomView,
                                            string roomFloor)
        {
            DbManager db = new DbManager();

            var roomID = db.GetRoomIDSelectListFromTypeViewFloor(Convert.ToDateTime(begDate), Convert.ToDateTime(endDate), roomType,
                                                                  roomView, roomFloor);

            return Json(roomID, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult CheckIfRoomIsFree(int? roomId, string begDate, string endDate)
        {
            DbManager db = new DbManager();

            var IsRoomFree = db.CheckIfRoomIsFree(roomId, Convert.ToDateTime(begDate), Convert.ToDateTime(endDate));

            bool result = false;

            if (IsRoomFree == 1)
                result = true;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }

}