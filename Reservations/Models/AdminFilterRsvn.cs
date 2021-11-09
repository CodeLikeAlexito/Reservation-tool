using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Reservations.Models
{
    public class AdminFilterRsvn
    {
        public DateTime? AccomodationDateFrom { get; set; }
        public DateTime? AccomodationDateTo { get; set; }
        public DateTime? LeaveDateFrom { get; set; }
        public DateTime? LeaveDateTo { get; set; }
        public StatusRsvn StatusRsvn { get; set; }
        public string ReservationID { get; set; }
        public string ReservationMaker { get; set; }
    }
}