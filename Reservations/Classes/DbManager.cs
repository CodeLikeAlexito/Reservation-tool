using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Oracle.ManagedDataAccess.Client;
using Reservations.Classes.Application.Home;
using Reservations.Classes.Utils;
using Reservations.Models;
using Reservations.Models.Queries;
using Reservations.ViewModels.Prices;
using Reservations.ViewModels.Reservation;

namespace Reservations.Classes
{
    public class DbManager : OracleDB
    {
        public int CheckIfEGNISValid(string egn)
        {

            OracleCommand cmd = new OracleCommand("", GetDBConnection());

            cmd.CommandType = CommandType.Text;

            cmd.BindByName = true;

            OracleParameter ret = new OracleParameter();
            ret.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(ret);

            cmd.CommandText = @"select s_checks.is_EGN(:str_inp) from dual";
            cmd.Parameters.Add(GetStringParameter(egn, "str_inp"));

            ret.Value = cmd.ExecuteScalar();
            closeDBConnection();

            var result = Convert.ToInt32(ret.Value.ToString());

            return result;

        }

        public void UpdatePricesTable(PricesTableModel pricesData)
        {
            string query = string.Format(@"UPDATE rsvn_nom_prices
                                            SET room_view = :room_view,
                                                person_type = :person_type,
                                                price = :price,
                                                tax = :tax,
                                                total = :total,
                                                season = :season,
                                                ucb = :ucb
                                            WHERE ID =  " + pricesData.ID);

            OracleCommand cmd = new OracleCommand("", GetDBConnection());
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = query;
            cmd.BindByName = true;

            cmd.Parameters.Add(GetCharParameter(pricesData.RoomView, "room_view"));
            cmd.Parameters.Add(GetCharParameter(pricesData.PersonType, "person_type"));
            cmd.Parameters.Add(GetDecimalParameter(pricesData.Price, "price"));
            cmd.Parameters.Add(GetDecimalParameter(pricesData.Tax, "tax"));
            cmd.Parameters.Add(GetDecimalParameter(pricesData.Total, "total"));
            cmd.Parameters.Add(GetCharParameter(pricesData.Season, "season"));
            cmd.Parameters.Add(GetCharParameter(pricesData.UCB, "ucb"));

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            closeDBConnection();
        }

        public PricesTableModel GetPricesByID(int? id)
        {
            PricesTableModel pricesModel = null;

            OracleCommand cmd = new OracleCommand("", GetDBConnection());
            cmd.CommandType = CommandType.Text;
            cmd.BindByName = true;

            cmd.CommandText = string.Format(@"select * from rsvn_nom_prices where id = :price_id");
            cmd.Parameters.Add(GetIntParameter(id, "price_id"));

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    pricesModel = new PricesTableModel();

                    pricesModel.ID = Tools.ConvertToInt(reader["id"]);
                    pricesModel.RoomView = Tools.ConvertToChar(reader["room_view"]);
                    pricesModel.PersonType = Tools.ConvertToChar(reader["person_type"]);
                    pricesModel.Price = Tools.ConvertToDecimal(reader["price"]);
                    pricesModel.Tax = Tools.ConvertToDecimal(reader["tax"]);
                    pricesModel.Total = Tools.ConvertToDecimal(reader["total"]);
                    pricesModel.Season = Tools.ConvertToChar(reader["season"]);
                    pricesModel.UCB = Tools.ConvertToChar(reader["ucb"]);

                    break;
                }
            }

            closeDBConnection();

            return pricesModel;
        }
        public IEnumerable<SelectListItem> GetDistinctRoomDesc()
        {
            List<SelectListItem> resultList = new List<SelectListItem>();

            OracleCommand cmd = new OracleCommand("", GetDBConnection());
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = string.Format(@"select distinct room_desc from rsvn_nom_rooms");

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    PricesTableModel item = new PricesTableModel
                    {
                        RoomView = Tools.ConvertToChar(reader["room_desc"])
                    };

                    resultList.Add(new SelectListItem { Selected = true, Text = Tools.GetPricesRoomViewDescrition(item.RoomView), Value = item.RoomView.ToString() });
                }
            }

            closeDBConnection();

            return resultList;
        }


        public IEnumerable<SelectListItem> GetDistinctPersonTypePricesTable()
        {
            List<SelectListItem> resultList = new List<SelectListItem>();

            OracleCommand cmd = new OracleCommand("", GetDBConnection());
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = string.Format(@"select distinct person_type from rsvn_nom_prices");

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    PricesTableModel item = new PricesTableModel
                    {
                        PersonType = Tools.ConvertToChar(reader["person_type"])
                    };

                    resultList.Add(new SelectListItem { Selected = true, Text = Tools.GetPricesPersonTypeDescrition(item.PersonType), Value = item.PersonType.ToString() });
                }
            }

            closeDBConnection();

            return resultList;
        }

        public IEnumerable<SelectListItem> GetDistinctSeason()
        {
            List<SelectListItem> resultList = new List<SelectListItem>();

            OracleCommand cmd = new OracleCommand("", GetDBConnection());
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = string.Format(@"select distinct season from rsvn_nom_periods");

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    PricesTableModel item = new PricesTableModel
                    {
                        Season = Tools.ConvertToChar(reader["season"])
                    };

                    resultList.Add(new SelectListItem { Selected = true, Text = Tools.GetPricesSeasonTypeDescrition(item.Season), Value = item.Season.ToString() });
                }
            }

            closeDBConnection();

            return resultList;
        }

        public IEnumerable<SelectListItem> GetDistinctUCBType()
        {
            List<SelectListItem> resultList = new List<SelectListItem>();

            OracleCommand cmd = new OracleCommand("", GetDBConnection());
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = string.Format(@"select distinct ucb from rsvn_nom_prices");

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    PricesTableModel item = new PricesTableModel
                    {
                        UCB = Tools.ConvertToChar(reader["ucb"])
                    };

                    resultList.Add(new SelectListItem { Selected = true, Text = Tools.GetPricesUcbTypeDescrition(item.UCB), Value = item.UCB.ToString() });
                }
            }

            closeDBConnection();

            return resultList;
        }


        public List<Querie1> GetQueriesQuery1()
        {
            List<Querie1> resultList = new List<Querie1>();

            OracleCommand cmd = new OracleCommand("", GetDBConnection());
            cmd.CommandType = CommandType.Text;
            cmd.BindByName = true;

            cmd.CommandText = string.Format(@"select res.date_rsvn, res.maker, res.date_payment, trunc(date_rsvn) + 5 last_payment_date, res.date_cancellation,
                                               res.date_accommodation, res.date_leaving,res.nights, res.total_price,
                                               guest.first_name, guest.last_name, guest.family_name, guest.person_type, guest.person_age, guest.egn, guest.birth_date,
                                               rm.room_level, rm.room_id, rm.room_type
                                               from rsvn_reservations res, rsvn_guest guest, rsvn_rooms rm
                                               where res.reservation_id = guest.reservation_id
                                               and res.reservation_id = rm.reservation_id
                                               order by res.date_rsvn");

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Querie1 item = new Querie1
                    {
                        RsvnDate = Tools.ConvertToDateTime(reader["DATE_RSVN"]),
                        RsvnMaker = Tools.ConvertToString(reader["MAKER"]),
                        PaymentDATE = Tools.ConvertToDateTime(reader["DATE_PAYMENT"]),
                        LastPaymentDATE = Tools.ConvertToDateTime(reader["LAST_PAYMENT_DATE"]),
                        CancellationDATE = Tools.ConvertToDateTime(reader["DATE_CANCELLATION"]),
                        AccommodationDATE = Tools.ConvertToDateTime(reader["DATE_ACCOMMODATION"]),
                        LeavingDATE = Tools.ConvertToDateTime(reader["DATE_LEAVING"]),
                        Nights = Tools.ConvertToDouble(reader["NIGHTS"]),
                        TotalPrice = Tools.ConvertToDecimal(reader["TOTAL_PRICE"]),
                        FirstName = Tools.ConvertToString(reader["FIRST_NAME"]),
                        LastName = Tools.ConvertToString(reader["LAST_NAME"]),
                        FamilyName = Tools.ConvertToString(reader["FAMILY_NAME"]),
                        PersonType = Tools.ConvertToChar(reader["PERSON_TYPE"]),
                        PersonAge = Tools.ConvertToChar(reader["PERSON_AGE"]),
                        EGN = Tools.ConvertToString(reader["EGN"]),
                        BirthDate = Tools.ConvertToDateTime(reader["BIRTH_DATE"]),
                        RoomFloor = Tools.ConvertToString(reader["ROOM_LEVEL"]),
                        RoomID = Tools.ConvertToInt(reader["ROOM_ID"]),
                        RoomType = Tools.ConvertToChar(reader["ROOM_TYPE"]),
                    };
                    resultList.Add(item);
                }
            }

            closeDBConnection();

            return resultList;

        }

        public List<Querie3> GetQueriesQuery3()
        {
            List<Querie3> resultList = new List<Querie3>();

            OracleCommand cmd = new OracleCommand("", GetDBConnection());
            cmd.CommandType = CommandType.Text;
            cmd.BindByName = true;

            cmd.CommandText = string.Format(@"select t.date_leaving, r.room_id
                                                from rsvn_reservations t, rsvn_rooms r
                                                where t.reservation_id = r.reservation_id
                                                order by t.date_leaving");

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Querie3 item = new Querie3
                    {
                        LeavingDate = Tools.ConvertToDateTime(reader["date_leaving"]),
                        RoomID = Tools.ConvertToInt(reader["room_id"])
                    };
                    resultList.Add(item);
                }
            }

            closeDBConnection();

            return resultList;

        }


        public List<Querie2> GetQueriesQuery2()
        {
            List<Querie2> resultList = new List<Querie2>();

            OracleCommand cmd = new OracleCommand("", GetDBConnection());
            cmd.CommandType = CommandType.Text;
            cmd.BindByName = true;

            cmd.CommandText = string.Format(@"select r.beg_date, r.end_date,c.room_view, count(*) as reserved_rooms,
                                                (select count(*) from rsvn_nom_rooms nr where nr.room_view = c.room_view) - count(*) as free_rooms
                                                from rsvn_rooms c, rsvn_reservations r, rsvn_reservations t
                                                where c.reservation_id = r.reservation_id
                                                and t.reservation_id = r.reservation_id
                                                and t.reservation_id =  c.reservation_id
                                                and r.beg_date = t.beg_date
                                                and r.end_date = r.end_date
                                                group by r.beg_date, r.end_date, c.room_view");

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Querie2 item = new Querie2
                    {
                        BegDate = Tools.ConvertToDateTime(reader["beg_date"]),
                        EndDate = Tools.ConvertToDateTime(reader["end_date"]),
                        RoomView = Tools.ConvertToChar(reader["room_view"]),
                        ReservedRoom = Tools.ConvertToInt(reader["reserved_rooms"]),
                        FreeRoom = Tools.ConvertToInt(reader["free_rooms"])
                    };
                    resultList.Add(item);
                }
            }

            closeDBConnection();

            return resultList;

        }


        public int CheckIfRoomIsFree(int? roomId, DateTime begDate, DateTime endDate)
        {
            
            OracleCommand cmd = new OracleCommand("", GetDBConnection());

            cmd.CommandType = CommandType.Text;

            cmd.BindByName = true;

            OracleParameter ret = new OracleParameter("v_result", OracleDbType.Int32);
            ret.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(ret);

            cmd.CommandText = @"select RSVN.CHECK_FREE_ROOM(:p_room_id, :p_beg_date, :p_end_date) from dual";
            cmd.Parameters.Add(GetIntParameter(roomId, "p_room_id"));
            cmd.Parameters.Add(GetNotNullableDateParameter(begDate, "p_beg_date"));
            cmd.Parameters.Add(GetNotNullableDateParameter(endDate, "p_end_date"));
            ret.Value = cmd.ExecuteScalar();
            closeDBConnection();

            var result = Convert.ToInt32(ret.Value.ToString());

            return result;

        }

        public void UpdateAll(Rsvn reservation, List<Room> roomList, List<Guest> guestList, UserInfo info)
        {
            UpdateReservationData(reservation,reservation.ID, info);

            foreach (Room room in roomList)
            {
                UpdateRoomData(room, room.Id);
            }

            foreach (Guest guest in guestList)
            {
                UpdateGuestData(guest, guest.GusetID);
            }
        }

        public void UpdateGuestData(Guest guest, int? guestId)
        {
            char personType = Tools.ConvertEnumPersonTypeToChar(guest.PersonType);
            char personAge = Tools.ConvertEnumPersonAgeToChar(guest.PersonAge);
            char personGender = Tools.ConvertEnumPersonGenderToChar(guest.Gender);

            string query = string.Format(@"UPDATE rsvn_guest
                                            SET first_name = :first_name,
                                            last_name = :last_name,
                                            family_name = :family_name,
                                            phone_number = :phone_number,
                                            person_type = :person_type,
                                            person_age = :person_age,
                                            gender = :gender,
                                            egn = :egn,
                                            birth_date = :birth_date,
                                            room_id = :room_id
                                            where reservation_id = '" + guest.ReservationID + "' and" +
                                            " guest_id = " + guestId );

            OracleCommand cmd = new OracleCommand("", GetDBConnection());
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = query;
            cmd.BindByName = true;

            cmd.Parameters.Add(GetIntParameter(guest.GusetID, "guest_id"));
            cmd.Parameters.Add(GetStringParameter(guest.FirstName, "first_name"));
            cmd.Parameters.Add(GetStringParameter(guest.LastName, "last_name"));
            cmd.Parameters.Add(GetStringParameter(guest.FamilyName, "family_name"));
            cmd.Parameters.Add(GetStringParameter(guest.PhoneNumber, "phone_number"));
            cmd.Parameters.Add(GetCharParameter(personType, "person_type"));
            cmd.Parameters.Add(GetCharParameter(personAge, "person_age"));
            cmd.Parameters.Add(GetCharParameter(personGender, "gender"));
            cmd.Parameters.Add(GetStringParameter(guest.EGN, "egn"));
            cmd.Parameters.Add(GetDateParameter(guest.BirthDate, "birth_date"));
            cmd.Parameters.Add(GetIntParameter(guest.GuestRoomID, "room_id"));

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            closeDBConnection();
        }

        public void UpdateRoomData(Room room, int? roomID)
        {
            string query = string.Format(@"UPDATE RSVN_ROOMS
                                            SET room_level =:room_level,
                                            room_view =:room_view,
                                            room_type =:room_type,
                                            room_id = :room_id
                                            WHERE reservation_id =  '" + room.ReservationId + "'" +
                                            "and id = " + roomID);

            OracleCommand cmd = new OracleCommand("", GetDBConnection());
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = query;
            cmd.BindByName = true;

            cmd.Parameters.Add(GetIntParameter(room.Id, "id"));
            cmd.Parameters.Add(GetStringParameter(room.RoomFloor, "room_level"));
            cmd.Parameters.Add(GetCharParameter(room.RoomView, "room_view"));
            cmd.Parameters.Add(GetCharParameter(room.RoomType, "room_type"));
            cmd.Parameters.Add(GetIntParameter(room.RoomID, "room_id"));

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            closeDBConnection();
        }

        public void UpdateReservationData(Rsvn reservation, int? Id,  UserInfo info)
        {
            char rsvnStatus = Tools.ConvertStatusReservationToChar(reservation.RsvnStatus);
            DateTime now = DateTime.Now;

            if (rsvnStatus == 'C')
                reservation.CancellationDATE = now;
            else
                reservation.CancellationDATE = null;
            

            string query = string.Format(@"UPDATE RSVN_RESERVATIONS
                                            SET total_price = :total_price,
                                              date_rsvn = :date_rsvn,
                                              maker = :maker,
                                              date_payment = :date_payment,
                                              date_cancellation = :date_cancellation,
                                              date_accommodation = :date_accommodation,
                                              date_leaving = :date_leaving,
                                              nights = :nights,
                                              beg_date = :beg_date,
                                              end_date = :end_date,
                                              status_rsvn = :status_rsvn,
                                              period = :period,
                                              adults = :adults,
                                              kids = :kids,
                                              room_number = :room_number,
                                              payment_comment = :payment_comment,
                                              date_rsvn_change = :date_rsvn_change,
                                              change_maker = :change_maker
                                            WHERE reservation_id =  '" + reservation.ReservationID + "' " +
                                            "and id = " + Id);

            OracleCommand cmd = new OracleCommand("", GetDBConnection());
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = query;
            cmd.BindByName = true;

            cmd.Parameters.Add(GetIntParameter(reservation.ID, "id"));
            cmd.Parameters.Add(GetDecimalParameter(reservation.TotalPrice, "total_price"));
            cmd.Parameters.Add(GetDateParameter(reservation.RsvnDATE, "date_rsvn"));
            cmd.Parameters.Add(GetStringParameter(reservation.RsvnMaker, "maker"));
            cmd.Parameters.Add(GetDateParameter(reservation.PaymentDATE, "date_payment"));
            cmd.Parameters.Add(GetDateParameter(reservation.CancellationDATE, "date_cancellation"));
            cmd.Parameters.Add(GetDateParameter(reservation.AccommodationDATE, "date_accommodation"));
            cmd.Parameters.Add(GetDateParameter(reservation.LeavingDATE, "date_leaving"));
            cmd.Parameters.Add(GetDoubleParameter(reservation.Nights, "nights"));
            cmd.Parameters.Add(GetDateParameter(reservation.BegDATE, "beg_date"));
            cmd.Parameters.Add(GetDateParameter(reservation.EndDATE, "end_date"));
            cmd.Parameters.Add(GetCharParameter(rsvnStatus, "status_rsvn"));
            cmd.Parameters.Add(GetIntParameter(reservation.Period, "period"));
            cmd.Parameters.Add(GetIntParameter(reservation.Adults, "adults"));
            cmd.Parameters.Add(GetIntParameter(reservation.Kids, "kids"));
            cmd.Parameters.Add(GetIntParameter(reservation.Rooms, "room_number"));
            cmd.Parameters.Add(GetStringParameter(reservation.Comment, "payment_comment"));
            cmd.Parameters.Add(GetDateParameter(now, "date_rsvn_change"));
            cmd.Parameters.Add(GetStringParameter(info.accessManager.BBUser, "change_maker"));

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            closeDBConnection();

        }

        public void UpdateReservationStatus(Rsvn reservation, UserInfo info)
        {
            DateTime now = DateTime.Now;
            char rsvnStatus = Tools.ConvertStatusReservationToChar(reservation.RsvnStatus);

            string query = string.Format(@"UPDATE RSVN_RESERVATIONS set
                                                   status_rsvn = :status,
                                                   change_maker = :bb_change,
                                                   date_rsvn_change = :bb_date_changed,
                                                   payment_comment = :rsvn_comment,
                                                   DATE_CANCELLATION = :date_cancellation
                                            WHERE reservation_id =  '" + reservation.ReservationID + "' ");

            OracleCommand cmd = new OracleCommand("", GetDBConnection());
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = query;
            cmd.BindByName = true;

            cmd.Parameters.Add(GetCharParameter(rsvnStatus, "status"));
            cmd.Parameters.Add(GetStringParameter(info.accessManager.BBUser, "bb_change"));
            cmd.Parameters.Add(GetDateParameter(now, "bb_date_changed"));
            cmd.Parameters.Add(GetDateParameter(now, "date_cancellation"));
            cmd.Parameters.Add(GetStringParameter(reservation.Comment, "rsvn_comment"));

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            closeDBConnection();
        }

        public Rsvn GetRsvnData(string reservation_id)// User role
        {
            Rsvn rsvn = null;

            OracleCommand cmd = new OracleCommand("", GetDBConnection());
            cmd.CommandType = CommandType.Text;
            cmd.BindByName = true;

            cmd.CommandText = string.Format(@"select * from rsvn_reservations r where r.reservation_id = :reservation_id");
            cmd.Parameters.Add(GetStringParameter(reservation_id, "reservation_id"));

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    rsvn = new Rsvn();

                    rsvn.ID = Tools.ConvertToInt(reader["id"]);
                    rsvn.ReservationID = reservation_id;
                    rsvn.RsvnStatus = Tools.ConvertCharStatusRsvnToEnum(reader["status_rsvn"]);
                    rsvn.BegDATE = Tools.ConvertToDateTime(reader["beg_date"]);
                    rsvn.EndDATE = Tools.ConvertToDateTime(reader["end_date"]);
                    rsvn.Nights = Tools.ConvertToDouble(reader["nights"]);
                    rsvn.RsvnDATE = Tools.ConvertToDateTime(reader["DATE_RSVN"]);
                    rsvn.AccommodationDATE = Tools.ConvertToDateTime(reader["DATE_ACCOMMODATION"]);
                    rsvn.LeavingDATE = Tools.ConvertToDateTime(reader["DATE_LEAVING"]);
                    rsvn.PaymentDATE = Tools.ConvertToDateTime(reader["DATE_PAYMENT"]);
                    rsvn.CancellationDATE = Tools.ConvertToDateTime(reader["DATE_CANCELLATION"]);
                    rsvn.Period = Tools.ConvertToInt(reader["period"]);
                    rsvn.PeriodList = new List<SelectListItem>();
                    rsvn.Adults = Tools.ConvertToInt(reader["adults"]);
                    rsvn.Kids = Tools.ConvertToInt(reader["kids"]);
                    rsvn.Rooms = Tools.ConvertToInt(reader["room_number"]);
                    rsvn.TotalPrice = Tools.ConvertToDecimal(reader["total_price"]);
                    rsvn.RsvnMaker = Tools.ConvertToString(reader["maker"]);
                    rsvn.PaymentRefNo = Tools.ConvertToString(reader["payment_ref_no"]);
                    rsvn.Comment = Tools.ConvertToString(reader["payment_comment"]);
                    
                    break;
                }
            }

            closeDBConnection();

            return rsvn;
        }

        public List<Room> GetAllRooms(string reservation_id)
        {
            List<Room> resultList = new List<Room>();

            OracleCommand cmd = new OracleCommand("", GetDBConnection());
            cmd.CommandType = CommandType.Text;


            cmd.CommandText = string.Format(@"select * from RSVN_ROOMS t where t.reservation_id =  :reservation_id");
            cmd.Parameters.Add(GetStringParameter(reservation_id, "reservation_id"));
            cmd.BindByName = true;


            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Room item = new Room
                    {
                        Id = Tools.ConvertToInt(reader["id"]),
                        ReservationId = Tools.ConvertToString(reader["reservation_id"]),
                        RoomFloor = Tools.ConvertToString(reader["room_level"]),
                        RoomID = Tools.ConvertToInt(reader["room_id"]),
                        RoomType = Tools.ConvertToChar(reader["room_type"]),
                        RoomView = Tools.ConvertToChar(reader["room_view"])
                    };
                    resultList.Add(item);
                }
            }

            closeDBConnection();

            return resultList;
        }

        public List<Guest> GetAllGuestsFromReservationId(string reservation_id)
        {
            List<Guest> resultList = new List<Guest>();

            OracleCommand cmd = new OracleCommand("", GetDBConnection());
            cmd.CommandType = CommandType.Text;
            

            cmd.CommandText = string.Format(@"select * from RSVN_GUEST d where d.reservation_id = :reservation_id");
            cmd.Parameters.Add(GetStringParameter(reservation_id, "reservation_id"));

            cmd.BindByName = true;

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Guest item = new Guest
                    {
                        GusetID = Tools.ConvertToInt(reader["guest_id"]),
                        ReservationID = Tools.ConvertToString(reader["reservation_id"]),
                        FirstName = Tools.ConvertToString(reader["first_name"]),
                        LastName = Tools.ConvertToString(reader["last_name"]),
                        FamilyName = Tools.ConvertToString(reader["family_name"]),
                        PhoneNumber = Tools.ConvertToString(reader["phone_number"]),
                        PersonType = Tools.ConvertCharPersonTypeToEnum(reader["person_type"]),
                        PersonAge = Tools.ConvertCharPersonAgeToEnum(reader["person_age"]),
                        Gender = Tools.ConvertCharPersonGenderToEnum(reader["gender"]),
                        EGN = Tools.ConvertToString(reader["egn"]),
                        BirthDate = Tools.ConvertToDateTime(reader["birth_date"]),
                        GuestRoomID = Tools.ConvertToInt(reader["room_id"])
                    };
                    resultList.Add(item);
                }
            }

            closeDBConnection();

            return resultList;
        }

        public List<Rsvn> GetAllReservations()
        {
            List<Rsvn> resultList = new List<Rsvn>();

            OracleCommand cmd = new OracleCommand("", GetDBConnection());
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = string.Format(@"select * from rsvn_reservations tt");

            
            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Rsvn item = new Rsvn
                    {
                        ID = Tools.ConvertToInt(reader["id"]),
                        ReservationID = Tools.ConvertToString(reader["reservation_id"]),
                        TotalPrice = Tools.ConvertToDecimal(reader["total_price"]),
                        RsvnDATE = Tools.ConvertToDateTime(reader["date_rsvn"]),
                        RsvnMaker = Tools.ConvertToString(reader["maker"]),
                        PaymentDATE = Tools.ConvertToDateTime(reader["date_payment"]),
                        CancellationDATE = Tools.ConvertToDateTime(reader["date_cancellation"]),
                        AccommodationDATE = Tools.ConvertToDateTime(reader["date_accommodation"]),
                        LeavingDATE = Tools.ConvertToDateTime(reader["date_leaving"]),
                        Nights = Tools.ConvertToDouble(reader["nights"]),
                        BegDATE = Tools.ConvertToDateTime(reader["beg_date"]),
                        EndDATE = Tools.ConvertToDateTime(reader["end_date"]),
                        RsvnStatus = Tools.ConvertCharStatusRsvnToEnum(reader["status_rsvn"]),
                        Period = Tools.ConvertToInt(reader["period"]),
                        Adults = Tools.ConvertToInt(reader["adults"]),
                        Kids = Tools.ConvertToInt(reader["kids"]),
                        Rooms = Tools.ConvertToInt(reader["room_number"]),
                        PaymentRefNo = Tools.ConvertToString(reader["payment_ref_no"]),
                        Comment = Tools.ConvertToString(reader["payment_comment"])
                    };
                    resultList.Add(item);
                }
            }

            closeDBConnection();

            return resultList;
        }

        /* Admin panel Filter */
        public List<Rsvn> GetAllReservationsFilter(DateTime? dateAccStart, DateTime? dateAccEnd, DateTime? leaveDateStart,
            DateTime? leaveDateEnd, StatusRsvn status, string maker, string reservationId)
        {
            List<Rsvn> resultList = new List<Rsvn>();

            OracleCommand cmd = new OracleCommand("", GetDBConnection());
            cmd.CommandType = CommandType.Text;
            cmd.BindByName = true;

            char? statusChar = null;

            if (status != 0)
                statusChar = Tools.ConvertStatusReservationToChar(status);


            cmd.CommandText = string.Format(@"select * from rsvn_reservations tt
                                             where (((tt.date_accommodation between nvl(:date_acc_start, to_date('01.01.1900', 'dd.mm.yyyy'))
                                              and nvl(:date_acc_end, to_date('01.01.1900', 'dd.mm.yyyy'))
                                               or tt.date_leaving between nvl(:date_leave_start, to_date('01.01.1900', 'dd.mm.yyyy'))
                                                and nvl(:date_leave_end, to_date('01.01.1900', 'dd.mm.yyyy')))
                                                 and tt.maker = nvl(:maker, tt.maker)
                                                  and tt.status_rsvn = nvl(:status_rsvn, tt.status_rsvn) 
                                                  and tt.reservation_id = nvl(:reservation_id, tt.reservation_id)))");

            cmd.Parameters.Add(GetDateParameter(dateAccStart, "date_acc_start"));
            cmd.Parameters.Add(GetDateParameter(dateAccEnd, "date_acc_end"));
            cmd.Parameters.Add(GetDateParameter(leaveDateStart, "date_leave_start"));
            cmd.Parameters.Add(GetDateParameter(leaveDateEnd, "date_leave_end"));
            cmd.Parameters.Add(GetCharParameter(statusChar, "status_rsvn"));
            cmd.Parameters.Add(GetStringParameter(maker, "maker"));
            cmd.Parameters.Add(GetStringParameter(reservationId, "reservation_id"));

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Rsvn item = new Rsvn
                    {
                        ReservationID = Tools.ConvertToString(reader["reservation_id"]),
                        TotalPrice = Tools.ConvertToDecimal(reader["total_price"]),
                        RsvnDATE = Tools.ConvertToDateTime(reader["date_rsvn"]),
                        RsvnMaker = Tools.ConvertToString(reader["maker"]),
                        AccommodationDATE = Tools.ConvertToDateTime(reader["date_accommodation"]),
                        LeavingDATE = Tools.ConvertToDateTime(reader["date_leaving"]),
                        Nights = Tools.ConvertToDouble(reader["nights"]),
                        RsvnStatus = Tools.ConvertCharStatusRsvnToEnum(reader["status_rsvn"]),
                    };
                    resultList.Add(item);
                }
            }

            closeDBConnection();

            return resultList;
        }


        public List<AvailableRooms> GroupAvailableRooms(DateTime? begDate, DateTime? endDate, int? maxPersons, int? numRooms)
        {
            List<AvailableRooms> resultList = new List<AvailableRooms>();

            OracleCommand cmd = new OracleCommand("", GetDBConnection());
            cmd.CommandType = CommandType.Text;
            cmd.BindByName = true;
            int _id = 0;

            cmd.CommandText = string.Format(@"select ro.room_view, ro.room_type, ro.max_persons, count(ro.room_id) as Free_Rooms, 
                                            (select count (*) as total from RSVN_NOM_ROOMS t
                                                    where t.room_view = ro.room_view 
                                                    and t.room_type = ro.room_type  
                                                    and t.max_persons = ro.max_persons
                                                    and (t.num_rooms = ro.num_rooms
                                                    or t.num_rooms < ro.num_rooms)) - count(ro.room_id) as reserved
                                            from rsvn_nom_rooms ro
                                            where ro.room_id not in ( select rr.room_id from rsvn_rooms rr
                                                                        where rr.reservation_id in 
                                                                          (
                                                                            select t.reservation_id from rsvn_reservations t
                                                                            where :beg_date between t.beg_date and t.end_date
                                                                            and :end_date between t.beg_date and t.end_date
                                                                            and t.status_rsvn = 'R'
                                                                          ) 
                                                                        )
                                            and ((ro.num_rooms = :num_rooms and ro.max_persons = :max_persons) or ( ro.num_rooms < :num_rooms and ro.max_persons <= :max_persons))
                                              or (ro.num_rooms = :num_rooms and :max_persons = 1 and ro.max_persons = 2)
                                                                            group by ro.room_view, ro.room_type, ro.max_persons, ro.num_rooms");

            cmd.Parameters.Add(GetDateParameter(begDate, "beg_date"));
            cmd.Parameters.Add(GetDateParameter(endDate, "end_date"));
            cmd.Parameters.Add(GetIntParameter(numRooms, "num_rooms"));
            cmd.Parameters.Add(GetIntParameter(maxPersons, "max_persons"));

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    AvailableRooms item = new AvailableRooms
                    {
                        Id = _id,
                        RoomView = Tools.ConvertToChar(reader["room_view"]),
                        RoomType = Tools.ConvertToChar(reader["room_type"]),
                        FreeRooms = Tools.ConvertToInt(reader["Free_Rooms"]),
                        ReservedRooms = Tools.ConvertToInt(reader["reserved"]),
                        MaxPeople = Tools.ConvertToInt(reader["max_persons"])
                    };
                    resultList.Add(item);
                    _id++;
                }
            }

            closeDBConnection();

            return resultList;

        }
        
        public List<PricesTableModel> GetPrices()
        {
            List<PricesTableModel> resultList = new List<PricesTableModel>();

            OracleCommand cmd = new OracleCommand("", GetDBConnection());
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = string.Format(@"select * from rsvn_nom_prices t");

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    PricesTableModel item = new PricesTableModel
                    {
                        ID = Tools.ConvertToInt(reader["id"]),
                        RoomView = Tools.ConvertToChar(reader["room_view"]),
                        PersonType = Tools.ConvertToChar(reader["person_type"]),
                        Price = Tools.ConvertToDecimal(reader["price"]),
                        Tax = Tools.ConvertToDecimal(reader["tax"]),
                        Total = Tools.ConvertToDecimal(reader["total"]),
                        Season = Tools.ConvertToChar(reader["season"]),
                        UCB = Tools.ConvertToChar(reader["ucb"])

                    };

                    resultList.Add(item);
                }
            }

            closeDBConnection();

            return resultList;
        }

        
        public List<RoomsTableModel> GetAllRoomsNomencalutereTable()
        {
            List<RoomsTableModel> resultList = new List<RoomsTableModel>();

            OracleCommand cmd = new OracleCommand("", GetDBConnection());
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = string.Format(@"select * from RSVN_NOM_ROOMS t");

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    RoomsTableModel item = new RoomsTableModel
                    {
                        RoomID = Tools.ConvertToInt(reader["room_id"]), 
                        RoomLevel = Tools.ConvertToString(reader["room_level"]),  
                        RoomView = Tools.ConvertToChar(reader["room_view"]),  
                        RoomType = Tools.ConvertToChar(reader["room_type"]),  
                        RoomDescription = Tools.ConvertToChar(reader["room_desc"]),  
                        MaxPeople = Tools.ConvertToInt(reader["max_persons"]),
                        NumRooms = Tools.ConvertToInt(reader["num_rooms"])
                    };

                    resultList.Add(item);
                }
            }

            closeDBConnection();

            return resultList;
        }
       
        public List<PeriodsTableModel> GetPeriod()
        {
            List<PeriodsTableModel> resultList = new List<PeriodsTableModel>();

            OracleCommand cmd = new OracleCommand("", GetDBConnection());
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = string.Format(@"select * from RSVN_NOM_PERIODS t");

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    PeriodsTableModel item = new PeriodsTableModel
                    {
                        PeriodID = Tools.ConvertToInt(reader["period_id"]),
                        Season = Tools.ConvertToChar(reader["season"]),
                        BeginDate = Tools.ConvertToDateTime(reader["beg_date"]),
                        EndDate = Tools.ConvertToDateTime(reader["end_date"])
                    };

                    resultList.Add(item);
                }
            }
            closeDBConnection();

            return resultList;
        }

        public List<PeriodsTableModel> GetStartEndDateOfPeriod(int? period_id)
        {
            List<PeriodsTableModel> resultList = new List<PeriodsTableModel>();

            OracleCommand cmd = new OracleCommand("", GetDBConnection());
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = string.Format(@"select c.beg_date, c.end_date from rsvn_nom_periods c where c.period_id = :period_id");
            cmd.Parameters.Add(GetIntParameter(period_id, "period_id"));

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    PeriodsTableModel item = new PeriodsTableModel
                    {
                        BeginDate = Tools.ConvertToDateTime(reader["beg_date"]),
                        EndDate = Tools.ConvertToDateTime(reader["end_date"])
                    };

                    resultList.Add(item);
                }
            }

            closeDBConnection();

            return resultList;
        }

        
        public List<RoomsTableModel> GetRoomView(char? room_type)
        {
            List<RoomsTableModel> resultList = new List<RoomsTableModel>();

            OracleCommand cmd = new OracleCommand("", GetDBConnection());
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = string.Format(@"select distinct c.room_view  from rsvn_nom_rooms c where c.room_type = :room_type");
            cmd.Parameters.Add(GetCharParameter(room_type, "room_type"));

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    RoomsTableModel item = new RoomsTableModel
                    {
                        RoomView = Tools.ConvertToChar(reader["room_view"])
                    };

                    resultList.Add(item);
                }
            }

            closeDBConnection();

            return resultList;
        }

        /* List of All Room Views */
        public IEnumerable<SelectListItem> GetRoomViewSelectList()
        {
            List<SelectListItem> resultList = new List<SelectListItem>();

            OracleCommand cmd = new OracleCommand("", GetDBConnection());
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = string.Format(@"select distinct c.room_view from rsvn_nom_rooms c order by c.room_view");
            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Room item = new Room
                    {
                        RoomView = Tools.ConvertToChar(reader["room_view"])
                    };
                    resultList.Add(new SelectListItem { Selected = false, Text = Tools.GetRoomRoomViewDescription(item.RoomView), Value = item.RoomView.ToString() });
                }
            }
            closeDBConnection();

            return resultList;
        }

        /* Room Floor selected list based on RoomType and RoomView */
        public IEnumerable<SelectListItem> GetRoomFloorSelectListFromRoomTypeView(char? room_type, char? room_view)
        {
            List<SelectListItem> resultList = new List<SelectListItem>();

            OracleCommand cmd = new OracleCommand("", GetDBConnection());
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = string.Format(@"select distinct a.room_level from rsvn_nom_rooms a where a.room_type = :room_type and a.room_view = :room_view");
            cmd.Parameters.Add(GetCharParameter(room_type, "room_type"));
            cmd.Parameters.Add(GetCharParameter(room_view, "room_view"));

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Room item = new Room
                    {
                        RoomFloor = Tools.ConvertToString(reader["room_level"])
                    };

                    resultList.Add(new SelectListItem { Selected = true, Text = Tools.GetRoomRoomFloorDescription(item.RoomFloor), Value = item.RoomFloor});
                }
            }

            closeDBConnection();

            return resultList;
        }

        /* Room View List based on RoomType */
        public IEnumerable<SelectListItem> GetRoomViewSelectListFromType(char? room_type)
        {
            List<SelectListItem> resultList = new List<SelectListItem>();

            OracleCommand cmd = new OracleCommand("", GetDBConnection());
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = string.Format(@"select distinct c.room_view from rsvn_nom_rooms c where c.room_type = :room_type order by c.room_view");
            cmd.Parameters.Add(GetCharParameter(room_type, "room_type"));
            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Room item = new Room
                    {
                        RoomView = Tools.ConvertToChar(reader["room_view"])
                    };

                    resultList.Add(new SelectListItem { Selected = true, Text = Tools.GetRoomRoomViewDescription(item.RoomView), Value = item.RoomView.ToString() });
                }
            }

            closeDBConnection();

            return resultList;
        }

        /* Get List of Room Ids based on RoomType, RoomView and RoomFloor */
        public IEnumerable<SelectListItem> GetRoomIDSelectListFromTypeViewFloor(DateTime begDate, DateTime endDate, char? room_type, char? room_view,
                                                                            string room_level)
        {
            List<SelectListItem> resultList = new List<SelectListItem>();
            //char rsvnStatus = 'C';

            OracleCommand cmd = new OracleCommand("", GetDBConnection());
            cmd.CommandType = CommandType.Text;
            cmd.BindByName = true;

            cmd.CommandText = string.Format(@"select ff.room_id from rsvn_nom_rooms ff
                                                where ff.room_id not in
                                                (
                                                select c.room_id from v_rsvn_reservation_details c
                                                      where c.beg_date = :beg_date and c.end_date = :end_date
                                                      or (c.beg_date between :beg_date and :end_date
                                                           and c.end_date between :beg_date and :end_date)
                                                      or (c.beg_date between :beg_date and :end_date
                                                           and c.end_date >= :end_date)
                                                      or (c.beg_date <= :beg_date
                                                           and c.end_date between :beg_date and :end_date)
                                                      or (c.beg_date <= :beg_date
                                                           and c.end_date >= :end_date)
                                                )
                                                and ff.room_type = :room_type
                                                and ff.room_view = :room_view
                                                and ff.room_level = :room_level");

            cmd.Parameters.Add(GetNotNullableDateParameter(begDate, "beg_date"));
            cmd.Parameters.Add(GetNotNullableDateParameter(endDate, "end_date"));
            cmd.Parameters.Add(GetCharParameter(room_type, "room_type"));
            cmd.Parameters.Add(GetCharParameter(room_view, "room_view"));
            cmd.Parameters.Add(GetStringParameter(room_level, "room_level"));
            //cmd.Parameters.Add(GetCharParameter(rsvnStatus, "rsvn_status"));

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Room item = new Room
                    {
                        RoomID = Tools.ConvertToInt(reader["room_id"])
                    };

                    resultList.Add(new SelectListItem { Selected = true, Text = item.RoomID.ToString(), Value = item.RoomID.ToString() });
                }
            }

            closeDBConnection();

            return resultList;
        }


        /* Get List of All Rooms */
        public IEnumerable<SelectListItem> GetAllRoomIDSelectList()
        {
            List<SelectListItem> resultList = new List<SelectListItem>();

            OracleCommand cmd = new OracleCommand("", GetDBConnection());
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = string.Format(@"select d.room_id from RSVN_NOM_ROOMS d");

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Room item = new Room
                    {
                        RoomID = Tools.ConvertToInt(reader["room_id"])
                    };
                    resultList.Add(new SelectListItem { Selected = false, Text = Tools.GetRoomRoomIDDescription(item.RoomID), Value = item.RoomID.ToString() });
                }
            }
            closeDBConnection();

            return resultList;
        }

        /* List of Room Types */
        public IEnumerable<SelectListItem> GetRoomTypeSelectList()
        {
            List<SelectListItem> resultList = new List<SelectListItem>();

            OracleCommand cmd = new OracleCommand("", GetDBConnection());
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = string.Format(@"select distinct t.room_type from rsvn_nom_rooms t");

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Room item = new Room
                    {
                        RoomType = Tools.ConvertToChar(reader["room_type"])
                    };
                    resultList.Add(new SelectListItem { Selected = false, Text = Tools.GetRoomRoomTypeDescription(item.RoomType), Value = item.RoomType.ToString() });
                }
            }
            closeDBConnection();

            return resultList;
        }

        /* List of Room Levels */
        public IEnumerable<SelectListItem> GetRoomLevelSelectList()
        {
            List<SelectListItem> resultList = new List<SelectListItem>();

            OracleCommand cmd = new OracleCommand("", GetDBConnection());
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = string.Format(@"select distinct d.room_level from RSVN_NOM_ROOMS d order by d.room_level");

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Room item = new Room
                    {
                        RoomFloor = Tools.ConvertToString(reader["room_level"])
                    };

                    resultList.Add(new SelectListItem { Selected = false, Text = Tools.GetRoomRoomFloorDescription(item.RoomFloor), Value = item.RoomFloor.ToString() });
                }
            }
            closeDBConnection();

            return resultList;
        }

        /* List of Periods for vacation */
        public IEnumerable<SelectListItem> GetPeriodSelectList()
        {
            List<SelectListItem> resultList = new List<SelectListItem>();

            OracleCommand cmd = new OracleCommand("", GetDBConnection());
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = string.Format(@"select t.period_id from RSVN_NOM_PERIODS t");

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Rsvn item = new Rsvn
                    {
                        Period = Tools.ConvertToInt(reader["period_id"])
                    };

                    resultList.Add(new SelectListItem { Selected = false, Text = Tools.GetPeriodPeriodIDDescription(item.Period), Value = item.Period.ToString() });
                }
            }

            closeDBConnection();

            return resultList;
        }

        /* Insert in rsvn_room table */
        public int InsertRoom(Room room, string reservation_id)
        {
            DateTime now = DateTime.Now;
            int nextVal = GetNextval("rsvn_room_seq_id");
            string query = @"INSERT INTO RSVN_ROOMS(
                                                    id,        
                                                    reservation_id,
                                                    room_level,
                                                    room_view,
                                                    room_type,
                                                    room_id,
                                                    flag_main_room
                                                    )
                                                    values
                                                    (
                                                    :id,        
                                                    :reservation_id,
                                                    :room_level,
                                                    :room_view,
                                                    :room_type,
                                                    :room_id,
                                                    :flag_main_room
                                                    )";
            OracleCommand cmd = new OracleCommand("", GetDBConnection());
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = query;
            cmd.BindByName = true;

            OracleParameter opid = new OracleParameter();
            opid.DbType = DbType.Int32;
            opid.Value = nextVal;
            opid.ParameterName = "id";
            cmd.Parameters.Add(opid);

            cmd.Parameters.Add(GetIntParameter(room.Id, "id"));
            cmd.Parameters.Add(GetStringParameter(reservation_id, "reservation_id"));
            cmd.Parameters.Add(GetStringParameter(room.RoomFloor, "room_level"));
            cmd.Parameters.Add(GetCharParameter(room.RoomView, "room_view"));
            cmd.Parameters.Add(GetCharParameter(room.RoomType, "room_type"));
            cmd.Parameters.Add(GetIntParameter(room.RoomID, "room_id"));
            cmd.Parameters.Add(GetCharParameter(room.IsMainRoom, "flag_main_room"));

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            closeDBConnection();

            return nextVal;

        }

        /* Insert in rsvn_guest table */
        public int InsertGuest(Guest guest, string reservation_id)
        {
            DateTime now = DateTime.Now;
            int nextVal = GetNextval("rsvn_guest_seq_id");
            string query = @"INSERT INTO RSVN_GUEST(
                                                    guest_id,        
                                                    reservation_id,
                                                    first_name,
                                                    last_name,
                                                    family_name,
                                                    phone_number,
                                                    person_type,
                                                    person_age,
                                                    gender,
                                                    egn,
                                                    birth_date,
                                                    room_id,
                                                    flag_main_guest
                                                    )
                                                    values
                                                    (
                                                    :guest_id,
                                                    :reservation_id,
                                                    :first_name,
                                                    :last_name,
                                                    :family_name,
                                                    :phone_number,
                                                    :person_type,
                                                    :person_age,
                                                    :gender,
                                                    :egn,
                                                    :birth_date,
                                                    :room_id,
                                                    :flag_main_guest
                                                    )";
            OracleCommand cmd = new OracleCommand("", GetDBConnection());
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = query;
            cmd.BindByName = true;

            OracleParameter opid = new OracleParameter();
            opid.DbType = DbType.Int32;
            opid.Value = nextVal;
            opid.ParameterName = "guest_id";
            cmd.Parameters.Add(opid);

            char personType = Tools.ConvertEnumPersonTypeToChar(guest.PersonType);
            char personAge = Tools.ConvertEnumPersonAgeToChar(guest.PersonAge);
            char personGender = Tools.ConvertEnumPersonGenderToChar(guest.Gender);

            cmd.Parameters.Add(GetIntParameter(guest.GusetID, "guest_id"));
            cmd.Parameters.Add(GetStringParameter(reservation_id, "reservation_id"));
            cmd.Parameters.Add(GetStringParameter(guest.FirstName, "first_name"));
            cmd.Parameters.Add(GetStringParameter(guest.LastName, "last_name"));
            cmd.Parameters.Add(GetStringParameter(guest.FamilyName, "family_name"));
            cmd.Parameters.Add(GetStringParameter(guest.PhoneNumber, "phone_number"));
            cmd.Parameters.Add(GetCharParameter(personType, "person_type"));
            cmd.Parameters.Add(GetCharParameter(personAge, "person_age"));
            cmd.Parameters.Add(GetCharParameter(personGender, "gender"));
            cmd.Parameters.Add(GetStringParameter(guest.EGN, "egn"));
            cmd.Parameters.Add(GetDateParameter(guest.BirthDate, "birth_date"));
            cmd.Parameters.Add(GetIntParameter(guest.GuestRoomID, "room_id"));
            cmd.Parameters.Add(GetCharParameter(guest.IsMainGuest, "flag_main_guest"));

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            closeDBConnection();

            return nextVal;

        }

        /* Insert in rsvn_reservation table */
        public int InsertReservation(Rsvn reservation, string reservation_id)
        {
            DateTime now = DateTime.Now;
            int nextVal = GetNextval("rsvn_seq_id");
            string query = @"INSERT INTO RSVN_RESERVATIONS(
                                                            ID,
                                                            RESERVATION_ID,
                                                            TOTAL_PRICE,
                                                            DATE_RSVN,
                                                            MAKER,
                                                            DATE_PAYMENT,
                                                            DATE_CANCELLATION,
                                                            DATE_ACCOMMODATION,
                                                            DATE_LEAVING,
                                                            NIGHTS,
                                                            BEG_DATE,
                                                            END_DATE,
                                                            STATUS_RSVN,
                                                            period,
                                                            adults,
                                                            kids,
                                                            room_number
                                                            )
                                                            values
                                                            (
                                                            :id,
                                                            :reservation_id,
                                                            :total_price,
                                                            :date_rsvn,
                                                            :maker,
                                                            :date_payment,
                                                            :date_cancellation,
                                                            :date_accommodation,
                                                            :date_leaving,
                                                            :nights,
                                                            :beg_date,
                                                            :end_date,
                                                            :status_rsvn,
                                                            :period,
                                                            :adults,
                                                            :kids,
                                                            :room_number
                                                            )";

            OracleCommand cmd = new OracleCommand("", GetDBConnection());
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = query;
            cmd.BindByName = true;

            OracleParameter opid = new OracleParameter();
            opid.DbType = DbType.Int32;
            opid.Value = nextVal;
            opid.ParameterName = "id";
            cmd.Parameters.Add(opid);

            char rsvnStatus = Tools.ConvertStatusReservationToChar(reservation.RsvnStatus);

            cmd.Parameters.Add(GetStringParameter(reservation_id, "reservation_id"));
            cmd.Parameters.Add(GetDecimalParameter(reservation.TotalPrice, "total_price"));
            cmd.Parameters.Add(GetDateParameter(reservation.RsvnDATE, "date_rsvn"));
            cmd.Parameters.Add(GetStringParameter(reservation.RsvnMaker, "maker"));
            cmd.Parameters.Add(GetDateParameter(reservation.PaymentDATE, "date_payment"));
            cmd.Parameters.Add(GetDateParameter(reservation.CancellationDATE, "date_cancellation"));
            cmd.Parameters.Add(GetDateParameter(reservation.AccommodationDATE, "date_accommodation"));
            cmd.Parameters.Add(GetDateParameter(reservation.LeavingDATE, "date_leaving"));
            cmd.Parameters.Add(GetDoubleParameter(reservation.Nights, "nights"));
            cmd.Parameters.Add(GetDateParameter(reservation.BegDATE, "beg_date"));
            cmd.Parameters.Add(GetDateParameter(reservation.EndDATE, "end_date"));
            cmd.Parameters.Add(GetCharParameter(rsvnStatus, "status_rsvn"));
            cmd.Parameters.Add(GetIntParameter(reservation.Period, "period"));
            cmd.Parameters.Add(GetIntParameter(reservation.Adults, "adults"));
            cmd.Parameters.Add(GetIntParameter(reservation.Kids, "kids"));
            cmd.Parameters.Add(GetIntParameter(reservation.Rooms, "room_number"));

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            closeDBConnection();

            return nextVal;
        }

        /* Insert all data for reservation (rsvn, room, guest) at once. */
        public int InsertAll(Rsvn rsvn, List<Room> roomList, List<Guest> guestList)
        {
            int nextVal = GetNextval("reservation_seq_id");
            string rsvnId = "R#" + nextVal.ToString().PadLeft(6, '0');

            int insrtRsvn = InsertReservation(rsvn, rsvnId);
            foreach (Room room in roomList)
            {
                int insrtRoom = InsertRoom(room, rsvnId);
            }

            foreach (Guest guest in guestList)
            {
                int insrtGuest = InsertGuest(guest, rsvnId);
            }
            

            return nextVal;
        }

    }
}