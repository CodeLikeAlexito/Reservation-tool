using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Reservations.Classes
{
    public class EnumDisplayNameAttribute : Attribute
    {
        private string _displayName;
        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; }
        }
    }

    public static class ExtensionMethods
    {
        public static System.Web.Mvc.SelectList ToSelectList<TEnum>(this TEnum obj)
            where TEnum : struct, IComparable, IFormattable, IConvertible // correct one
        {

            return new SelectList(Enum.GetValues(typeof(TEnum)).OfType<Enum>()
                .Select(x =>
                    new SelectListItem
                    {
                        Text = x.DisplayName(),
                        Value = (Convert.ToInt32(x)).ToString()
                    }), "Value", "Text");
        }

        public static string DisplayName(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());

            EnumDisplayNameAttribute attribute
                    = Attribute.GetCustomAttribute(field, typeof(EnumDisplayNameAttribute))
                        as EnumDisplayNameAttribute;

            return attribute == null ? value.ToString() : attribute.DisplayName;
        }
    }
}