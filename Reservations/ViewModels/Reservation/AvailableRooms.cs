using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reservations.ViewModels.Reservation
{
    public class AvailableRooms
    {
        public int? Id { get; set; }
        public string RoomFloor { get; set; }
        public int? RoomID { get; set; }
        public char? RoomType { get; set; }
        public char? RoomView { get; set; }
        public int? MaxPeople { get; set; }
        public int? FreeRooms { get; set; }
        public int? ReservedRooms { get; set; }
        public int? NumRooms { get; set; }

        public string RoomFloorDescription { get; set; }
        public string RoomTypeDescription { get; set; }
        public string RoomViewDescription { get; set; }
    }
}