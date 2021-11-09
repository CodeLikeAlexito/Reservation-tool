using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Reservations.ViewModels.Admin
{
    public class QueryOneVM
    {
        [DisplayName("Дата резервация")]
        [Display(Name = "Дата резервация")]
        public string RsvnDateDescription { get; set; }
        [DisplayName("Направил резервацията")]
        [Display(Name = "Направил резервацията")]
        public string RsvnMaker { get; set; }
        [DisplayName("Дата на плащане")]
        [Display(Name = "Дата на плащане")]
        public string PaymentDATEDescription { get; set; }
        [DisplayName("Крайна дата за плащане")]
        [Display(Name = "Крайна дата за плащане")]
        public string LastPaymentDATEDescription { get; set; }
        [DisplayName("Дата на анулиране на резервация")]
        [Display(Name = "Дата на анулиране на резервация")]
        public string CancellationDATEDescription { get; set; }
        [DisplayName("Дата настаняване")]
        [Display(Name = "Дата настаняване")]
        public string AccommodationDATEDescription { get; set; }
        [DisplayName("Дата напускане")]
        [Display(Name = "Дата напускане")]
        public string LeavingDATEDescription { get; set; }
        [DisplayName("Брой нощувки")]
        [Display(Name = "Брой нощувки")]
        public double Nights { get; set; }
        [DisplayName("Име")]
        [Display(Name = "Име")]
        public string FirstName { get; set; }
        [DisplayName("Презиме")]
        [Display(Name = "Презиме")]
        public string LastName { get; set; }
        [DisplayName("Фамилия")]
        [Display(Name = "Фамилия")]
        public string FamilyName { get; set; }
        [DisplayName("Тип служител")]
        [Display(Name = "Тип служител")]
        public string PersonTypeDescription { get; set; }
        [DisplayName("Възраст")]
        [Display(Name = "Възраст")]
        public string PersonAgeDescription { get; set; }
        [DisplayName("ЕГН")]
        [Display(Name = "ЕГН")]
        public string EGN { get; set; }
        [DisplayName("Дата на раждане")]
        [Display(Name = "Дата на раждане")]
        public string BirthDateDescription { get; set; }
        [DisplayName("Етаж")]
        [Display(Name = "Етаж")]
        public string RoomFloorDescription { get; set; }
        [DisplayName("Номер на стая")]
        [Display(Name = "Номер на стая")]
        public int? RoomID { get; set; }
        [DisplayName("Вид стая")]
        [Display(Name = "Вид стая")]
        public string RoomTypeDescription { get; set; }




        

    }
}