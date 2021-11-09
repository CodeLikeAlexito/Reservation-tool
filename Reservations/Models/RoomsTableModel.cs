using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reservations.Models
{
    public class RoomsTableModel
    {
        public int? RoomID { get; set; }
        public string RoomLevel { get; set; }
        public char? RoomView { get; set; }
        public  char? RoomType { get; set; }
        public char? RoomDescription { get; set; }
        public int? MaxPeople { get; set; }
        public int? NumRooms { get; set; }
        public int? NumGuests { get; set; }
    }
}