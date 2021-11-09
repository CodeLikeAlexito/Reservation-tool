using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reservations.Classes.Utils
{
    public static class Helper
    {
        public static bool IsAny<T>(this IEnumerable<T> data)
        {
            return data != null && data.Any();
        }
    }
}