using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reservations.Models.Queries
{
    public class Querie2
    {
        public DateTime? BegDate { get; set; }
        public DateTime? EndDate { get; set; }
        public char? RoomView { get; set; }
        public int? ReservedRoom { get; set; }
        public int? FreeRoom { get; set; }
    }
}