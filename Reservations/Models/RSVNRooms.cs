using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reservations.Models
{
    public static class RoomsRoomLevelType
    {
        public const string PARTER = "P";
        public const string MIDLEVEL_MINUS_ONE = "M";
        public const string MIDLEVEL_ONE = "Z";
        public const string MIDLEVEL_TWO = "Q";

    }

    public static class RoomsRoomViewType
    {
        public const char SEE = 'A';
        public const char STREET = 'B';
        public const char SEE_TERRACE = 'C';
        public const char SEE_TERRACE_BED = 'D';
        public const char YARD_TERRACE = 'E';
        public const char YARD_BED = 'F';
        public const char STREET_TERRACE = 'G';
        public const char STREET_TERRACE_BED = 'H';
        

    }

    public static class RoomsRoomTypeType
    {
        public const char DOUBLE = 'D';
        public const char TRIPLE = 'T';
        public const char LRG_APT = 'L';
        public const char SML_APT = 'S';
    }

    public static class RoomsRoomDescType
    {
        public const char SEE = 'O';
        public const char STREET = 'I';
        public const char LRG_APT = 'L';
        public const char SML_APT = 'S';
    }
}