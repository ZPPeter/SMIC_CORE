using System;
using System.Collections.Generic;
using System.Text;
namespace SMIC.Web.Host.Hubs
{
    public class ConnectionManager
    {
        public static List<ConnectionUser> ConnectionUsers { get; set; } = new List<ConnectionUser>();
    }
}
