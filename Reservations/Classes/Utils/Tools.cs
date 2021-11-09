using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Reservations.Models;

namespace Reservations.Classes.Utils
{
    public class Tools
    {
        public static SelectList SetSelectedValue(SelectList list, string value)
        {
            if (value != null)
            {
                var selected = list.Where(x => x.Value == value).First();
                selected.Selected = true;
                return list;
            }
            return list;
        }

        public static char? CheckIfRoomIsWithAdditionalBed(char? c)
        {
            switch(c)
            {
                case 'A': return 'N';
                case 'B': return 'N';
                case 'C': return 'N';
                case 'D': return 'Y';
                case 'E': return 'N';
                case 'F': return 'Y';
                case 'G': return 'N';
                case 'H': return 'Y';
            }

            return null;
        }

        public static string GetPersonTypeForQueriesDescription(char e)
        {
            switch (e)
            {
                case 'Y': return "Служител";
                case 'N': return "Външен";
            }

            return "";
        }

        public static string ConvertDateTimeNullableToStringRsvnDate(DateTime? date)
        {
            DateTime dateNotNullable;

            if (date == null)
            {
                return "";
            }
            else
            {
                dateNotNullable = (DateTime)date;
            }
                
                

            return String.Format("{0:dd/MM/yyyy HH:MM:ss}", dateNotNullable);
        }

        public static string ConvertDateTimeNullableToString(DateTime? date)
        {
            DateTime dateNotNullable;

            if (date == null)
            {
                return "";
            }
            else
            {
                dateNotNullable = (DateTime)date;
            }

            return String.Format("{0:dd/MM/yyyy}", dateNotNullable);
        }

        public static string PriceDescription(decimal? price)
        {
            decimal pp = (decimal)price; 
            return pp.ToString("#.00 лв."); ;
        }

        public static char GetUCB(char e)
        {
            /* if person is PensionerUCB than return Y */
            if (e == 'P')
                return 'Y';

            /* Else return Y or N */
            return e;
        }
        
        public static char GetPersonType(char e)
        {
            if (e == 'B')
                return 'K';


            return e;    
        }

        public static IEnumerable<SelectListItem> EnumToSelectList<PersonAge>(char c)
        {
            return (Enum.GetValues(typeof(PersonAge)).Cast<PersonAge>().Select(
               e => new SelectListItem () { Text = Tools.GetPersonAgeEnumDescription(c), Value = e.ToString() })).ToList();
        }

        public static string StatusReservationDescription(char? status)
        {
            switch (status)
            {
                case 'R': return "Плащане";
                case 'A': return "Потвърдена";
                case 'S': return "Частично платена";
                case 'C': return "Анулирана";

            }

            return "";
        }

        public static string GetPersonTypeEnumDescription(char e)
        {
            switch (e)
            {
                case 'Y': return "Служител";
                case 'P': return "Пенсионер";
                case 'N': return "Външен";

            }

            return "";
        }

        public static string GetPersonAgeEnumDescription(char e)
        {
            switch (e)
            {
                case 'A': return "Възрастен";
                case 'K': return "Дете от 3 до 12 години";
                case 'B': return "Дете од 0 до 3 години";

            }

            return "";
        }

        public static string GetGenderEnumDescription(char e)
        {
            switch (e)
            {
                case 'M': return "Мъж";
                case 'F': return "Жена";

            }

            return "";
        }

        public static string GetRsvnStatusEnumDescription(char e)
        {
            switch (e)
            {
                case 'R': return "Плащане";
                case 'A': return "Потвърдена";
                case 'S': return "Частично платена";
                case 'C': return "Анулирана";

            }

            return "";
        }

        public static string GetRoomRoomViewDescription(char? room_view)
        {
            switch (room_view)
            {
                case RoomsRoomViewType.SEE: return "Море";
                case RoomsRoomViewType.SEE_TERRACE: return "Море и тераса";
                case RoomsRoomViewType.SEE_TERRACE_BED: return "Море и тераса с дополнително легло";
                case RoomsRoomViewType.STREET: return "Улица";
                case RoomsRoomViewType.YARD_TERRACE: return "Двор и тераса";
                case RoomsRoomViewType.YARD_BED: return "Двор с дополнително легло";
                case RoomsRoomViewType.STREET_TERRACE: return "Улица и тераса";
                case RoomsRoomViewType.STREET_TERRACE_BED: return "Улица и тераса с дополнително легло";
            }

            return "";
        }

        public static string GetRoomRoomTypeDescription(char? room_type)
        {
            switch (room_type)
            {
                case RoomsRoomTypeType.DOUBLE: return "Двойка";
                case RoomsRoomTypeType.TRIPLE: return "Тройка";
                case RoomsRoomTypeType.LRG_APT: return "Голям апартамент";
                case RoomsRoomTypeType.SML_APT: return "Малък апартамент";
            }
            return "";
        }

        public static string GetRoomRoomFloorDescription(string room_floor)
        {
            switch(room_floor)
            {
                case RoomsRoomLevelType.MIDLEVEL_MINUS_ONE: return "Полуниво -1";
                case RoomsRoomLevelType.PARTER: return "Партер";
                case RoomsRoomLevelType.MIDLEVEL_ONE: return "Полуниво 1";
                case RoomsRoomLevelType.MIDLEVEL_TWO: return "Полуниво 2";
            }

            return room_floor;
        }

        public static string GetRoomRoomIDDescription(int? room_id)
        {
            string roomID = room_id.ToString();

            return roomID;
        }

        public static string GetPeriodPeriodIDDescription(int? Id)
        {
            switch(Id)
            {
                case PeriodsPeriodID.FIRST_PERIOD: return "1 смяна - (1 юни - 10 юни)";
                case PeriodsPeriodID.SECOND_PERIOD: return "2 смяна - (12 юни - 21 юни)";
                case PeriodsPeriodID.THIRD_PERIOD: return "3 смяна - (23 юни - 2 юли)";
                case PeriodsPeriodID.FOURTH_PERIOD: return "4 смяна - (4 юли - 13 юли)";
                case PeriodsPeriodID.FIFTH_PERIOD: return "5 смяна - (15 юли - 24 юли)";
                case PeriodsPeriodID.SIXT_PERIOD: return "6 смяна - (26 юли - 4 август)";
                case PeriodsPeriodID.SEVENT_PERIOD: return "7 смяна - (6 август - 15 август)";
                case PeriodsPeriodID.EIGHT_PERIOD: return "8 смяна - (17 август - 26 август)";
                case PeriodsPeriodID.NINTH_PERIOD: return "9 смяна - (28 август - 6 септември)";
                case PeriodsPeriodID.TENTH_PERIOD: return "10 смяна - (8 септември - 17 септември)";
                case PeriodsPeriodID.ELEVENTH_PERIOD: return "11 смяна - (19 септември - 28 септември)";
            }

            return "";
        }

        public static string GetPricesRoomViewDescrition(char? status)
        {
            switch (status)
            {
                case PricesRoomView.LARGE_APT: return "Голям апартамент";
                case PricesRoomView.SEA: return "Море";
                case PricesRoomView.SMALL_APT: return "Малък Апартамент";
                case PricesRoomView.STREET: return "Улица"; 
            }

            return "";
        }

        public static string GetPricesPersonTypeDescrition(char? status)
        {
            switch (status)
            {
                case PricesPersonType.ADULT: return "Възрастен";
                case PricesPersonType.CHILDREN: return "Дете";
                case PricesPersonType.ALL: return "Всички";

            }

            return "";
        }

        public static string GetPricesSeasonTypeDescrition(char? status)
        {
            switch (status)
            {
                case PricesSeasonType.HIGH: return "Висок";
                case PricesSeasonType.LOW: return "Нисък";
            }

            return "";
        }

        public static string GetPricesUcbTypeDescrition(char? status)
        {
            switch (status)
            {
                case PricesUcbType.UCB: return "Служител";
                case PricesUcbType.NOT_UCB: return "Външен";
            }

            return "";
        }

        public static bool ConvertCharToBool(object value)
        {
            bool result = false;

            if (value is DBNull || value == null)
                return false;

            string all_day_event = Tools.ConvertToString(value);

            if (all_day_event == "Y")
                result = true;
            else
                result = false;

            return result;
        }


        public static float? ConvertToFloat(object value)
        {
            float? result = null;

            if (value is DBNull || value == null)
                return result;

            try
            {
                result = Convert.ToSingle(value);
            }
            catch (Exception e) { }

            return result;
        }

        public static decimal? ConvertToDecimal(object value)
        {
            decimal? result = null;

            if (value is DBNull || value == null)
                return result;

            try
            {
                result = Convert.ToDecimal(value);
            }
            catch (Exception e) { }

            return result;
        }

        public static double ConvertToDouble(object value)
        {
            double result = 0;

            if (value is DBNull || value == null)
                return result;

            try
            {
                result = Convert.ToInt32(value);
            }
            catch (Exception e) { }

            return result;
        }

        public static int? ConvertToInt(object value)
        {
            int? result = null;

            if (value is DBNull || value == null)
                return result;

            try
            {
                result = Convert.ToInt32(value);
            }
            catch (Exception e) { }

            return result;
        }


        public static DateTime? ConvertToDateTime(object value)
        {
            DateTime? date = null;

            if (value is DBNull || value == null)
                return date;

            try
            {
                date = Convert.ToDateTime(value);
            }
            catch (Exception e) { }

            return date;
        }

        public static string ConvertToString(object value)
        {
            string result = null;

            if (value is DBNull || value == null)
                return result;

            try
            {
                result = Convert.ToString(value);
            }
            catch (Exception e) { }

            return result;
        }

        public static char ConvertEnumPersonTypeToChar(PersonType enumPersonType)
        {
            char charPersonType = (char)enumPersonType;

            return charPersonType;
        }

        public static char ConvertStatusReservationToChar(StatusRsvn enumRsvnStatus)
        {
            char charRsvnStatus = (char)enumRsvnStatus;

            return charRsvnStatus;
        }

        public static StatusRsvn ConvertCharStatusRsvnToEnum(object charStatusRsvn)
        {
            char? result = Convert.ToChar(charStatusRsvn);
            StatusRsvn enumStatusRsvn = (StatusRsvn)result;

            return enumStatusRsvn;
        }

        public static PersonType ConvertCharPersonTypeToEnum(object charPersonType)
        {
            char? result = Convert.ToChar(charPersonType);
            PersonType enumPersonType = (PersonType)result;

            return enumPersonType;
        }

        public static PersonAge ConvertCharPersonAgeToEnum(object charPersonAge)
        {
            char? result = Convert.ToChar(charPersonAge);
            PersonAge enumPersonAge = (PersonAge)result;

            return enumPersonAge;
        }

        public static Gender ConvertCharPersonGenderToEnum(object charPersonGender)
        {
            char? result = Convert.ToChar(charPersonGender);
            Gender enumPersonGender = (Gender)result;

            return enumPersonGender;
        }

        public static char ConvertEnumPersonAgeToChar(PersonAge enumPersonAge)
        {
            char charPersonAge = (char)enumPersonAge;

            return charPersonAge;
        }

        public static char ConvertEnumPersonGenderToChar(Gender enumPersonGender)
        {
            char charPersonGender = (char)enumPersonGender;

            return charPersonGender;
        }

        public static char? ConvertToChar(object value)
        {
            char? result = null;

            if (value is DBNull || value == null)
                return result;

            try
            {
                result = Convert.ToChar(value);
            }
            catch (Exception e) { }

            return result;
        }

        public static bool ConvertToBool(object value)
        {
            bool result = false;

            if (value is DBNull || value == null)
                return result;

            try
            {
                string res = Convert.ToString(value);

                if (res == "Y" || res == "y")
                    result = true;
            }
            catch (Exception e) { }

            return result;
        }
    }
}