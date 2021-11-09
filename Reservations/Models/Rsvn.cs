using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Reservations.Models
{
    public enum StatusRsvn
    {
        [Display(Name = "Плащане")]
        Payment = 'R',
        [Display(Name = "Потвърдена")]
        Accepted = 'A',
        [Display(Name = "Частично платена")]
        PartialPayment = 'S',
        [Display(Name = "Анулирана")]
        Cancelled = 'C'
    }

    public class Rsvn
    {
        public int? ID { get; set; }
        public string ReservationID { get; set; }
        public decimal? TotalPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [DataType(DataType.Date)]
        public DateTime? RsvnDATE { get; set; }
        public string RsvnMaker { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [DataType(DataType.Date)]
        public DateTime? PaymentDATE { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [DataType(DataType.Date)]
        public DateTime? CancellationDATE { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [DataType(DataType.Date)]
        public DateTime? AccommodationDATE { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? LeavingDATE { get; set; }
        public double Nights { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Въведете дата")]
        public DateTime? BegDATE { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [Required(ErrorMessage = "Въведете дата")]
        public DateTime? EndDATE { get; set; }
        public StatusRsvn RsvnStatus { get; set; }
        public int? Period { get; set; }
        [Required(ErrorMessage = "Брои възрастни")]
        public int? Adults { get; set; }
        [Required(ErrorMessage = "Брои деца")]
        public int? Kids { get; set; }
        [Required(ErrorMessage = "Брои стаи")]
        public int? Rooms { get; set; }
        public string PaymentRefNo { get; set; }
        public string Comment { get; set; }
        public string RsvnStatusDescription { get; set; }
        public string PeriodIDDescription { get; set; }
        public IEnumerable<SelectListItem> PeriodList { get; set; }

        public string RsvnDATEDescription { get; set; }
        public string PaymentDATEDescription { get; set; }
        public string CancellationDATEDescription { get; set; }
        public string AccommodationDATEDescription { get; set; }
        public string LeavingDATEDescription { get; set; }
        public string BegDATEDescription { get; set; }
        public string EndDATEDescription { get; set; }

    }
}