using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Reservations.Classes.Utils
{
    public static class ModelStateExtensions
    {
        public static Tuple<string, string> GetFirstError(this ModelStateDictionary modelState)
        {
            if (modelState.IsValid)
            {
                return null;
            }

            foreach (var key in modelState.Keys)
            {
                if (modelState[key].Errors.Count != 0)
                {
                    return new Tuple<string, string>(key, modelState[key].Errors[0].ErrorMessage);
                }
            }

            return null;
        }
    }
}