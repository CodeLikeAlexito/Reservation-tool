using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Reservations.Models
{
    public class PricesTableModel
    {
        public  int? ID { get; set; }
        [Required(ErrorMessage = "Изберете описание на стая")]
        public char? RoomView { get; set; }
        [Required(ErrorMessage = "Изберете възраст на гост")]
        public char? PersonType { get; set; }
        [Required(ErrorMessage ="Въведете цена")]
        public decimal? Price { get; set; }
        [Required(ErrorMessage = "Въведете такса")]
        public decimal? Tax { get; set; }
        [Required(ErrorMessage = "Въведете обща цена")]
        public decimal? Total { get; set; }
        [Required(ErrorMessage = "Изберете сезон")]
        public char? Season { get; set; }
        [Required(ErrorMessage = "Изберете служител")]
        public char? UCB { get; set; }

        public string RoomViewDescription { get; set; }
        public string PersonTypeDescription { get; set; }
        public string SeasonDescription { get; set; }
        public string UCBDescription { get; set; }
        public string PriceDescription { get; set; }
        public string TaxDescription { get; set; }
        public string TotalDescription { get; set; }

    }
}