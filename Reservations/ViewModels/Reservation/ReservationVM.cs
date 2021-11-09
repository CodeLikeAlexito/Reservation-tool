using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Reservations.Models;

namespace Reservations.ViewModels.Reservation
{
    public class ReservationVM
    {
        public Rsvn ReservationInfo { get; set; } // Model for Reservation
        public AdminFilterRsvn FilterAdmin { get; set; } // Model for Filter in Admin Panel ???
        public IEnumerable<Rsvn> ReservationQuery { get; set; } // Query of All Reservations in Database
        public IEnumerable<AvailableRooms> AvailableRooms { get; set; } // Query of All Free and Reserved Rooms SearchResult View
        public List<Room> RoomList { get; set; } // List of Rooms
        public List<Guest> GuestList { get; set; } // List of Guests

    }
}