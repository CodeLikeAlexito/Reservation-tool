using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Reservations.ViewModels.Admin
{
    public class QueryThreeVM
    {
        [DisplayName("Дата")]
        [Display(Name = "Дата")]
        public string LeavingDateDescription { get; set; }
        [DisplayName("Освобождаваща се стая №")]
        [Display(Name = "Освобождаваща се стая №")]
        public int? RoomID { get; set; }
    }
}