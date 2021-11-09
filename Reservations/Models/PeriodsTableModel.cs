using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reservations.Models
{
    public class PeriodsTableModel
    {
        public int? PeriodID { get; set; }
        public char? Season { get; set; }

        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}