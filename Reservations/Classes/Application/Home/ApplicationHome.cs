using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Reservations.Classes.Utils;
using Reservations.Models;
using Reservations.Models.Queries;
using Reservations.ViewModels.Admin;
using Reservations.ViewModels.Prices;
using Reservations.ViewModels.Reservation;

namespace Reservations.Classes.Application.Home
{
    public class ApplicationHome
    {
        private static DbManager db = new DbManager();
        private List<PricesTableModel> PricesTable = db.GetPrices();

        public ApplicationHome()
        {

        }

        /* Getting price with tax for one night in HIGH season depending on room view, person type and if it is UCB or not */
        public decimal? GetPriceForSingleNightHighWithTax(char? roomView, char personType, char ucb)
        {
            var GetPrice = (from price in PricesTable
                            where price.RoomView.Equals(roomView)
                            && price.PersonType.Equals(personType)
                            && price.Season == 'H'
                            && price.UCB.Equals(ucb)
                            select new { price.Total }).Single();


            if (GetPrice == null)
                return 0;

            return GetPrice.Total;
        }


        /* Getting price withоut tax for one night in HIGH season depending on room view, person type and if it is UCB or not. We use this method in case of additional bed */
        public decimal? GetPriceForSingleNightHigh(char? roomView, char personType, char ucb)
        {
            var GetPrice = (from price in PricesTable
                            where price.RoomView.Equals(roomView)
                            && price.PersonType.Equals(personType)
                            && price.Season == 'H'
                            && price.UCB.Equals(ucb)
                            select new { price.Price }).Single();

            if (GetPrice == null)
                return 0;

            return GetPrice.Price;
        }

        /* Getting price with tax for one night in LOW season depending on room view, person type and if it is UCB or not */
        public decimal? GetPriceForSingleNightLowWithTax(char? roomView, char personType, char ucb)
        {
            var GetPrice = (from price in PricesTable
                            where price.RoomView.Equals(roomView)
                            && price.PersonType.Equals(personType)
                            && price.Season == 'L'
                            && price.UCB.Equals(ucb)
                            select new { price.Total }).Single();

            if (GetPrice == null)
                return 0;

            return GetPrice.Total;
        }

        /* Getting price withоut tax for one night in LOW season depending on room view, person type and if it is UCB or not. We use this method in case of additional bed */
        public decimal? GetPriceForSingleNightLow(char? roomView, char personType, char ucb)
        {
            var GetPrice = (from price in PricesTable
                            where price.RoomView.Equals(roomView)
                            && price.PersonType.Equals(personType)
                            && price.Season == 'L' 
                            && price.UCB.Equals(ucb)
                            select new { price.Price }).Single();

            if (GetPrice == null)
                return 0;

            return GetPrice.Price;
        }

        public decimal? CalculateReservationPrice(Rsvn EnteredReservation, List<Room> EnteredRooms, List<Guest> EnteredGuests)
        {
            DateTime HighSeasonStart = new DateTime(DateTime.Now.Year, 6, 23); //Начало на висок сезон (hardcoded, needs to change every season)
            DateTime HighSeasonEnd = new DateTime(DateTime.Now.Year, 9, 6); // Край на висок сезон (hardcoded, needs to change every season)

            decimal? TotalPrice = 0;
            int DaysInHighSeason = 0;
            int DaysInLowSeason = 0;
            double VacationDays = ((DateTime)EnteredReservation.EndDATE - (DateTime)EnteredReservation.BegDATE).TotalDays + 1; // Брой дни за почивка
            DateTime CheckDate = (DateTime)EnteredReservation.BegDATE; // Дата на настаняване
            var taxQuery = (from prices in PricesTable
                            select new { prices.Tax }).Distinct().Single();
            var tax = taxQuery.Tax;

            /* Колко от дните са в висок, а колко са ви низък сезон */
            for (int i = 0; i < VacationDays; i++)
            {
                if (CheckDate >= HighSeasonStart && CheckDate <= HighSeasonEnd)
                    DaysInHighSeason++;
                else
                    DaysInLowSeason++;

                CheckDate = CheckDate.AddDays(1);
            }

            var AllRoomNomTableList = db.GetAllRoomsNomencalutereTable(); // Списък от всички стаи в почивната станция
            var RoomIdsOfGuests = new List<RoomsTableModel>(); // Списък с всички стаи за една резервация в която гостуват гостите (Някои стаи тук може да се повтарят, защото един или повече гости могат да споделят една стая)

            /* Пълним RoomIdsOfGuests */
            foreach (var guest in EnteredGuests)
            {
                var RoomNomTableList = from room in AllRoomNomTableList
                                       where room.RoomID == guest.GuestRoomID
                                       select room;

                if (RoomNomTableList != null)
                {
                    foreach (var item in RoomNomTableList)
                    {
                        RoomIdsOfGuests.Add(item);
                    }
                }
            }

            var ReservationRoomIDs = new List<RoomsTableModel>(); // Списък с номерата на стаите за една резервация
            ReservationRoomIDs.AddRange(RoomIdsOfGuests.Distinct()); // Тук са само уникални

            foreach (var item in ReservationRoomIDs)
            {
                var countGuests = RoomIdsOfGuests.Where(x => x.RoomID == item.RoomID).Count();
                item.NumGuests = countGuests; // Пълним property NumGuests of model Room, за да знаем точно колко човека има в определена стая ( можеби трябва да се направи при въвеждането ) 
            }

            /* Пълним масив с типовете гости. Ползваме този масив при изчисление на цената, когато стаята е с допълнителни легло. */
            char[] guestsTypes = new char[EnteredGuests.Count];
            for (int index = 0; index < guestsTypes.Length; index++)
            {
                guestsTypes[index] = (char)EnteredGuests[index].PersonAge;
            }


            for (int i = 0; i < ReservationRoomIDs.Count; i++)
            {
                var roomViewE = (from room in ReservationRoomIDs
                                 where room.RoomID == ReservationRoomIDs[i].RoomID
                                 select new { roomView = room.RoomDescription }).Single();

                char personAgeC;
                char personTypeC;
                decimal? highS = 0;
                decimal? lowS = 0;
                decimal? priceGuest = 0;

                /* If it is apartment */
                if (ReservationRoomIDs[i].RoomType == 'L'
                    || ReservationRoomIDs[i].RoomType == 'S')
                {
                    personTypeC = Tools.GetUCB(Tools.ConvertEnumPersonTypeToChar(EnteredGuests[0].PersonType));
                    highS = GetPriceForSingleNightHigh(roomViewE.roomView, 'E', personTypeC);
                    lowS = GetPriceForSingleNightLow(roomViewE.roomView, 'E', personTypeC);
                    priceGuest = ((highS * DaysInHighSeason) + (lowS * DaysInLowSeason));

                    var taxForAllGuests = (tax * (int)VacationDays) * EnteredGuests.Count;


                    TotalPrice += priceGuest + taxForAllGuests;
                    break;
                }
                /* Price for rooms */
                else
                {
                    for (int j = 0; j < EnteredGuests.Count; j++)
                    {
                        if (EnteredGuests[j].GuestRoomID.Equals(ReservationRoomIDs[i].RoomID))
                        {
                            personAgeC = Tools.GetPersonType(Tools.ConvertEnumPersonAgeToChar(EnteredGuests[j].PersonAge));
                            var personAgeCheck = Tools.ConvertEnumPersonAgeToChar(EnteredGuests[j].PersonAge);
                            personTypeC = Tools.GetUCB(Tools.ConvertEnumPersonTypeToChar(EnteredGuests[j].PersonType));
                            decimal? priceKidInAddBed = 0;

                            var roomTypeAddBedOrNot = Tools.CheckIfRoomIsWithAdditionalBed(ReservationRoomIDs[i].RoomView);

                            /* Цена за сам човек в стая */
                            if (ReservationRoomIDs[i].NumGuests == 1)
                            {
                                var highSAddBed = GetPriceForSingleNightHighWithTax(roomViewE.roomView, personAgeC, personTypeC);
                                var lowSAddBed = GetPriceForSingleNightLowWithTax(roomViewE.roomView, personAgeC, personTypeC);
                                var priceAddBed = ((highSAddBed * DaysInHighSeason) + (lowSAddBed * DaysInLowSeason));
                                priceAddBed /= 2;

                                TotalPrice += priceAddBed;
                                break;
                            }

                            if (roomTypeAddBedOrNot == 'N') // if room is not with additional bed
                            {
                                if (personAgeCheck == 'B') // That means that the guest is baby from 0 - 3 years old
                                {
                                    priceKidInAddBed = tax * (int)VacationDays;
                                    priceGuest = 0;
                                }
                                else if (personAgeCheck == 'K')
                                {
                                    highS = GetPriceForSingleNightHighWithTax(roomViewE.roomView, personAgeC, personTypeC);
                                    lowS = GetPriceForSingleNightLowWithTax(roomViewE.roomView, personAgeC, personTypeC);
                                    priceGuest = ((highS * DaysInHighSeason) + (lowS * DaysInLowSeason));
                                    //priceGuest /= 2;
                                }
                                else
                                {
                                    highS = GetPriceForSingleNightHighWithTax(roomViewE.roomView, personAgeC, personTypeC);
                                    lowS = GetPriceForSingleNightLowWithTax(roomViewE.roomView, personAgeC, personTypeC);
                                    priceGuest = ((highS * DaysInHighSeason) + (lowS * DaysInLowSeason));
                                }

                                TotalPrice += priceGuest + priceKidInAddBed;
                            }
                            else // If room is with addional bed
                            {
                                // Addults pay 50% of the bed price, plus daily tax

                                decimal? addBed = 0;

                                highS = GetPriceForSingleNightHighWithTax(roomViewE.roomView, personAgeC, personTypeC);
                                lowS = GetPriceForSingleNightLowWithTax(roomViewE.roomView, personAgeC, personTypeC);
                                priceGuest = ((highS * DaysInHighSeason) + (lowS * DaysInLowSeason));

                                if (j == EnteredGuests.Count - 1)
                                {
                                    if (guestsTypes.Contains('K') || guestsTypes.Contains('B'))
                                    {
                                        addBed += tax * (int)VacationDays;
                                        priceGuest = 0;
                                    }
                                    else
                                    {
                                        highS = GetPriceForSingleNightHigh(roomViewE.roomView, personAgeC, personTypeC);
                                        lowS = GetPriceForSingleNightLow(roomViewE.roomView, personAgeC, personTypeC);
                                        addBed = ((highS * DaysInHighSeason) + (lowS * DaysInLowSeason));
                                        addBed /= 2;
                                        addBed += tax * (int)VacationDays;
                                        priceGuest = 0;
                                    }
                                }                              
                                TotalPrice += priceGuest + addBed;
                            }
                        }
                    }
                } 
            }

            return TotalPrice;
        }

        public List<Rsvn> GetAllReservationsFilterDescription(DateTime? dateAccStart, DateTime? dateAccEnd, DateTime? leaveDateStart,
                                                              DateTime? leaveDateEnd, StatusRsvn status, string maker, string reservationId)
        {
            List<Rsvn> AllReservationsDb = new List<Rsvn>();
            List<Rsvn> AllReservationsDescr;
            DbManager dbManager = new DbManager();

            AllReservationsDb = dbManager.GetAllReservationsFilter(dateAccStart, dateAccEnd,
                                                    leaveDateStart, leaveDateEnd,
                                                    status, maker,
                                                    reservationId);

            AllReservationsDescr = (from tt in AllReservationsDb
                                    select new Rsvn
                                    {
                                        ID = tt.ID,
                                        ReservationID = tt.ReservationID,
                                        TotalPrice = tt.TotalPrice,
                                        RsvnDATEDescription = Tools.ConvertDateTimeNullableToStringRsvnDate(tt.RsvnDATE),
                                        RsvnMaker = tt.RsvnMaker,
                                        PaymentDATEDescription = Tools.ConvertDateTimeNullableToStringRsvnDate(tt.PaymentDATE),
                                        CancellationDATEDescription = Tools.ConvertDateTimeNullableToStringRsvnDate(tt.CancellationDATE),
                                        AccommodationDATEDescription = Tools.ConvertDateTimeNullableToStringRsvnDate(tt.AccommodationDATE),
                                        LeavingDATEDescription = Tools.ConvertDateTimeNullableToStringRsvnDate(tt.LeavingDATE),
                                        Nights = tt.Nights,
                                        BegDATEDescription = Tools.ConvertDateTimeNullableToString(tt.BegDATE),
                                        EndDATEDescription = Tools.ConvertDateTimeNullableToStringRsvnDate(tt.EndDATE),
                                        RsvnStatus = tt.RsvnStatus,
                                        RsvnStatusDescription = Tools.StatusReservationDescription((char)tt.RsvnStatus),
                                        Period = tt.Period,
                                        Adults = tt.Adults,
                                        Kids = tt.Kids,
                                        Rooms = tt.Rooms,
                                        PaymentRefNo = tt.PaymentRefNo,
                                        Comment = tt.Comment

                                    }).ToList();

            return AllReservationsDescr;
        }

        public List<Rsvn> GetAllReservationsDescription()
        {
            List<Rsvn> AllReservationsDb = new List<Rsvn>();
            List<Rsvn> AllReservationsDescr;
            DbManager dbManager = new DbManager();

            AllReservationsDb = dbManager.GetAllReservations();

            AllReservationsDescr = (from tt in AllReservationsDb
                                    select new Rsvn
                                    {
                                        ID = tt.ID,
                                        ReservationID = tt.ReservationID,
                                        TotalPrice = tt.TotalPrice,
                                        RsvnDATE = tt.RsvnDATE,
                                        RsvnDATEDescription = Tools.ConvertDateTimeNullableToStringRsvnDate(tt.RsvnDATE),
                                        RsvnMaker = tt.RsvnMaker,
                                        PaymentDATE = tt.PaymentDATE,
                                        PaymentDATEDescription = Tools.ConvertDateTimeNullableToString(tt.PaymentDATE),
                                        CancellationDATE = tt.CancellationDATE,
                                        CancellationDATEDescription = Tools.ConvertDateTimeNullableToString(tt.CancellationDATE),
                                        AccommodationDATE = tt.AccommodationDATE,
                                        AccommodationDATEDescription = Tools.ConvertDateTimeNullableToString(tt.AccommodationDATE),
                                        LeavingDATE = tt.LeavingDATE,
                                        LeavingDATEDescription = Tools.ConvertDateTimeNullableToString(tt.LeavingDATE),
                                        Nights = tt.Nights,
                                        BegDATE = tt.BegDATE,
                                        BegDATEDescription = Tools.ConvertDateTimeNullableToString(tt.BegDATE),
                                        EndDATE = tt.EndDATE,
                                        EndDATEDescription = Tools.ConvertDateTimeNullableToString(tt.EndDATE),
                                        RsvnStatus = tt.RsvnStatus,
                                        RsvnStatusDescription = Tools.StatusReservationDescription((char)tt.RsvnStatus),
                                        Period = tt.Period,
                                        Adults = tt.Adults,
                                        Kids = tt.Kids,
                                        Rooms = tt.Rooms,
                                        PaymentRefNo = tt.PaymentRefNo,
                                        Comment = tt.Comment

                                    }).ToList();

            return AllReservationsDescr;
        }

        public List<PricesTableModel> GetRoomsByRsvnIdDescription()
        {
            List<PricesTableModel> pricesTableList = new List<PricesTableModel>();
            List<PricesTableModel> pricesVMs;
            DbManager dbManager = new DbManager();

            pricesTableList = dbManager.GetPrices();

            pricesVMs = (from tt in pricesTableList
                         select new PricesTableModel
                         {
                             PersonTypeDescription = Tools.GetPricesPersonTypeDescrition(tt.PersonType),
                             Price = tt.Price,
                             Tax = tt.Tax,
                             Total = tt.Total,
                             SeasonDescription = Tools.GetPricesSeasonTypeDescrition(tt.Season),
                             UCBDescription = Tools.GetPricesUcbTypeDescrition(tt.UCB),
                             RoomViewDescription = Tools.GetPricesRoomViewDescrition(tt.RoomView)

                         }).ToList();

            return pricesVMs;
        }

        public List<PricesTableModel> GetAllPrices()
        {
            List<PricesTableModel> pricesTableList = new List<PricesTableModel>();
            List<PricesTableModel> pricesVMs;
            DbManager dbManager = new DbManager();

            pricesTableList = dbManager.GetPrices();

            pricesVMs = (from tt in pricesTableList
                         select new PricesTableModel
                         {
                             ID = tt.ID,
                             PersonTypeDescription = Tools.GetPricesPersonTypeDescrition(tt.PersonType),
                             PriceDescription = Tools.PriceDescription(tt.Price),
                             TaxDescription = Tools.PriceDescription(tt.Tax),
                             TotalDescription = Tools.PriceDescription(tt.Total),
                             SeasonDescription = Tools.GetPricesSeasonTypeDescrition(tt.Season),
                             UCBDescription = Tools.GetPricesUcbTypeDescrition(tt.UCB),
                             RoomViewDescription = Tools.GetPricesRoomViewDescrition(tt.RoomView)

                         }).ToList();

            return pricesVMs;
        }

        public List<QueryOneVM> GetQuery1Data()
        {
            DbManager db = new DbManager();
            List<Querie1> query1 = db.GetQueriesQuery1();

            var queryOneVM = (from tt in query1
                              select new QueryOneVM
                              {
                                  RsvnDateDescription = Tools.ConvertDateTimeNullableToStringRsvnDate(tt.RsvnDate),
                                  RsvnMaker = tt.RsvnMaker,
                                  PaymentDATEDescription = Tools.ConvertDateTimeNullableToString(tt.PaymentDATE),
                                  LastPaymentDATEDescription = Tools.ConvertDateTimeNullableToString(tt.LastPaymentDATE),
                                  CancellationDATEDescription = Tools.ConvertDateTimeNullableToString(tt.CancellationDATE),
                                  AccommodationDATEDescription = Tools.ConvertDateTimeNullableToString(tt.AccommodationDATE),
                                  LeavingDATEDescription = Tools.ConvertDateTimeNullableToString(tt.LeavingDATE),
                                  Nights = tt.Nights,
                                  FirstName = tt.FirstName,
                                  LastName = tt.LastName,
                                  FamilyName = tt.FamilyName,
                                  PersonTypeDescription = Tools.GetPersonTypeEnumDescription((char)tt.PersonType),
                                  PersonAgeDescription = Tools.GetPersonAgeEnumDescription((char)tt.PersonAge),
                                  EGN = tt.EGN,
                                  BirthDateDescription = Tools.ConvertDateTimeNullableToString(tt.BirthDate),
                                  RoomFloorDescription = Tools.GetRoomRoomFloorDescription(tt.RoomFloor),
                                  RoomID = tt.RoomID,
                                  RoomTypeDescription = Tools.GetRoomRoomTypeDescription(tt.RoomType)

                              }).ToList();

            return queryOneVM;
        }

        public List<QueryTwoVM> GetQuery2Data()
        {
            DbManager db = new DbManager();
            List<Querie2> query2 = db.GetQueriesQuery2();

            var queryTwoVM = (from tt in query2
                              select new QueryTwoVM
                              {
                                  BegDateDescription = Tools.ConvertDateTimeNullableToString(tt.BegDate),
                                  EndDateDescription = Tools.ConvertDateTimeNullableToString(tt.EndDate),
                                  RoomViewDescription = Tools.GetRoomRoomViewDescription(tt.RoomView),
                                  ReservedRoom = tt.ReservedRoom,
                                  FreeRoom = tt.FreeRoom

                              }).ToList();

            return queryTwoVM;
        }

        public List<QueryThreeVM> GetQuery3Data()
        {
            DbManager db = new DbManager();
            List<Querie3> query3 = db.GetQueriesQuery3();

            var queryThreeVM = (from tt in query3
                              select new QueryThreeVM
                              {
                                  LeavingDateDescription = Tools.ConvertDateTimeNullableToString(tt.LeavingDate),
                                  RoomID = tt.RoomID

                              }).ToList();

            return queryThreeVM;
        }

        public IEnumerable<AvailableRooms> GetGroupAvailableRoomsDescription(ReservationVM r)
        {
            List<AvailableRooms> GroupAvailableRoomsDescription = new List<AvailableRooms>();
            IEnumerable<AvailableRooms> listDesc;
            DbManager dbManager = new DbManager();
            int? maxPersons = r.ReservationInfo.Adults + r.ReservationInfo.Kids;

            GroupAvailableRoomsDescription = dbManager.GroupAvailableRooms(r.ReservationInfo.BegDATE, r.ReservationInfo.EndDATE, maxPersons, r.ReservationInfo.Rooms);


            listDesc = (from tt in GroupAvailableRoomsDescription
                        select new AvailableRooms
                        {
                            Id = tt.Id,
                            RoomViewDescription = Tools.GetRoomRoomViewDescription(tt.RoomView),
                            RoomTypeDescription = Tools.GetRoomRoomTypeDescription(tt.RoomType),
                            RoomView = tt.RoomView,
                            RoomType = tt.RoomType,
                            FreeRooms = tt.FreeRooms,
                            ReservedRooms = tt.ReservedRooms,
                            MaxPeople = tt.MaxPeople
                        }).ToList();

            return listDesc;
        }

        public IEnumerable<AvailableRooms> GetRoomViewDescription(char? room_type)
        {
            List<RoomsTableModel> GetRoomViewDescription = new List<RoomsTableModel>();
            IEnumerable<AvailableRooms> listDesc;
            DbManager dbManager = new DbManager();

            GetRoomViewDescription = dbManager.GetRoomView(room_type);

            listDesc = (from tt in GetRoomViewDescription
                        select new AvailableRooms
                        {
                            RoomViewDescription = Tools.GetRoomRoomViewDescription(tt.RoomView)
                        }).ToList();

            return listDesc;
        }

        public List<Rsvn> GetAllPeriods()
        {
            List<PeriodsTableModel> periodsTableList = new List<PeriodsTableModel>();
            List<Rsvn> periodsVMs;
            DbManager dbManager = new DbManager();

            periodsTableList = dbManager.GetPeriod();

            periodsVMs = (from tt in periodsTableList
                          select new Rsvn
                          {
                              PeriodIDDescription = Tools.GetPeriodPeriodIDDescription(tt.PeriodID)
                          }).ToList();

            return periodsVMs;
        }


    }
}