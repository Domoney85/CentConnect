using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CentConnect.Models
{
    public static class SessionInfo
    {
        public static int TempCharID {get; set;}
        public static int TempCampID { get; set; }
        public static string TempGMPass { get; set; }
        public static int SumAccount { get; set; }
        public static string errorFundMessage { get; set; }
    }
}