using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Reservations.Models.Queries
{
    public class Querie1
    {
        public DateTime? RsvnDate { get; set; }
        public string RsvnMaker { get; set; }
        public DateTime? PaymentDATE { get; set; }
        public DateTime? LastPaymentDATE { get; set; }
        public DateTime? CancellationDATE { get; set; }
        public DateTime? AccommodationDATE { get; set; }
        public DateTime? LeavingDATE { get; set; }
        public double Nights { get; set; }
        public decimal? TotalPrice { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FamilyName { get; set; }
        public char? PersonType { get; set; }
        public char? PersonAge { get; set; }
        public string EGN { get; set; }
        public DateTime? BirthDate { get; set; }
        public string RoomFloor { get; set; }
        public int? RoomID { get; set; }
        public char? RoomType { get; set; }
    }
}