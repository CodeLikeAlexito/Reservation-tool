using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Reservations.Classes;

namespace Reservations.Models
{
    public enum Gender
    {
        [Display(Name = "Мъж")]
        Male = 'M',
        [Display(Name = "Жена")]
        Female = 'F'
    }
    public enum PersonType
    {
        [Display(Name = "Служител")]
        PersonUCB = 'Y',
        [Display(Name = "Пенсиониран служител")]
        Pensioner = 'P',
        [Display(Name = "Външен")]
        PersonOUT = 'N'
    }
    public enum PersonAge
    {
        [Display(Name = "Възрастен")]
        Adult = 'A',
        [Display(Name = "Дете от 3 до 12 години")]
        Kid = 'K',
        [Display(Name = "Дете от 0 до 3 години")]
        Baby = 'B'
    }
    public class Guest
    {
        public int? GusetID { get; set; }
        public string ReservationID { get; set; }
        [Required(ErrorMessage = "Въведете име")]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required(ErrorMessage = "Въведете семейно име")]
        public string FamilyName { get; set; }
        public string PhoneNumber { get; set; }
        [EnumDataType(typeof(PersonType), ErrorMessage = "Въведете тип гост")]
        public PersonType PersonType { get; set; }
        [EnumDataType(typeof(PersonAge), ErrorMessage = "Въведете възраст")]
        public PersonAge PersonAge { get; set; }
        [EnumDataType(typeof(Gender), ErrorMessage = "Въведете пол")]
        public Gender Gender { get; set; }
        [Required(ErrorMessage = "Полето ЕГН не може да бъде празно")]
        [Remote("IsCorrectEGN", "Reservation", HttpMethod ="POST", ErrorMessage = "Въведете правилно ЕГН.")]
        public string EGN { get; set; }
        [Required(ErrorMessage = "Въведете дата на раждане")]
        [Remote("CheckPersonAge", "Reservation", HttpMethod = "POST", ErrorMessage = "Възраста която въведохте, не съответсва с тип гост.")]
        public DateTime? BirthDate { get; set; }
        [Required(ErrorMessage ="Въведете номер на стая")]
        public int? GuestRoomID { get; set; }
        public char? IsMainGuest { get; set; }

        public string PersonTypeDescription { get; set; }
        public string PersonAgeDescription { get; set; }
        public string PersonGenderDescription { get; set; }
        public string BirthDateDescription { get; set; }

        public IEnumerable<SelectListItem> PersonTypeList { get; set; }
        public IEnumerable<SelectListItem> PersonAgeList { get; set; }
        public IEnumerable<SelectListItem> PersonGenderList { get; set; }
        public IEnumerable<SelectListItem> GuestRoomIDList { get; set; }
    }
}