using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Reservations.ViewModels.Prices
{
    public class PricesTableVM
    {
        public int? ID { get; set; }
        public char? RoomView { get; set; }
        public char? PersonAge { get; set; }
        [Display(Name = "Цена")]
        public decimal? Price { get; set; }
        public decimal? Tax { get; set; }
        public decimal? Total { get; set; }
        public char? Season { get; set; }
        public char? UCB { get; set; }
    }
}