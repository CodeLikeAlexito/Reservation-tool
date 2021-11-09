using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reservations.Classes.Utils
{
    public enum UserRole
    {
        REGULAR_USER,
        POWER_USER,
        HELP_DESK,
        NONE
    }
    public class UserInfo
    {
        public UserInfo()
        {
            Role = UserRole.NONE;
        }

        public UCB.SecurityManager.AccessManager accessManager;

        public UserRole Role;
    }
}