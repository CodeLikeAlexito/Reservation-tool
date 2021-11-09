using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reservations.Models
{
    public  static class PricesRoomView
    {
        public const char SEA = 'O';
        public const char STREET = 'I';
        public const char SMALL_APT = 'S';
        public const char LARGE_APT = 'L';
    }

    public static class PricesPersonType
    {
        public const char ADULT = 'A';
        public const char CHILDREN = 'K';
        public const char ALL = 'E';
    }

    public static class PricesSeasonType
    {
        public const char LOW = 'L';
        public const char HIGH = 'H';
    }

    public static class PricesUcbType
    {
        public const char UCB = 'Y';
        public const char NOT_UCB = 'N';
    }
}