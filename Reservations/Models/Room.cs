using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Reservations.Classes;
using System.Web.Mvc;

namespace Reservations.Models
{
    public class Room
    {
        public int? Id { get; set; }
        public string ReservationId { get; set; }
        [Required(ErrorMessage = "Изберете етаж")]
        public string RoomFloor { get; set; }
        [Required(ErrorMessage = "Изберете номер на стая")]
        public int? RoomID { get; set; }
        [Required(ErrorMessage = "Изберете тип стая")]
        public char? RoomType { get; set; }
        [Required(ErrorMessage = "Изберете описание на стая")]
        public char? RoomView { get; set; }
        public int? MaxPeople { get; set; }
        public int? NumRooms { get; set; }
        public char? IsMainRoom { get; set; }
        public string RoomTypeDescription { get; set; }
        public string RoomViewDescription { get; set; }
        public string RoomFloorDescription { get; set; }
        public string RoomIDDescription { get; set; }

        public IEnumerable<SelectListItem> RoomIDList { get; set; }
        public IEnumerable<SelectListItem> RoomTypeList { get; set; }
        public IEnumerable<SelectListItem> RoomViewList { get; set; }
        public IEnumerable<SelectListItem> RoomFloorList { get; set; }
    }
}