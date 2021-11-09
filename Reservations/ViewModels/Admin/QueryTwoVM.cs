using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Reservations.ViewModels.Admin
{
    public class QueryTwoVM
    {
        [DisplayName("Начало")]
        [Display(Name = "Начало")]
        public string BegDateDescription { get; set; }
        [DisplayName("Край")]
        [Display(Name = "Край")]
        public string EndDateDescription { get; set; }
        [DisplayName("Заети стаи")]
        [Display(Name = "Заети стаи")]
        public string RoomViewDescription { get; set; }
        [DisplayName("Свободни стаи")]
        [Display(Name = "Свободни стаи")]
        public int? ReservedRoom { get; set; }
        [DisplayName("Описание")]
        [Display(Name = "Описание")]
        public int? FreeRoom { get; set; }
    }
}