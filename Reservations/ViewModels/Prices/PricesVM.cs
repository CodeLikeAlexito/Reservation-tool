using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Reservations.Models;

namespace Reservations.ViewModels.Prices
{
    public class PricesVM
    {
        public PricesTableModel PricesModel { get; set; }

        [Display(Name = "Описание на стая")]
        public IEnumerable<SelectListItem> RoomViewList { get; set; }
        public IEnumerable<SelectListItem> PersonTypeList { get; set; }
        public IEnumerable<SelectListItem> SeasonList { get; set; }
        public IEnumerable<SelectListItem> UCBList { get; set; }
    }
}