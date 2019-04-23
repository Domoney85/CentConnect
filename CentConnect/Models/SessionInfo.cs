using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using CentConnect.Models;
using System.Security.Claims;

namespace CentConnect.Models
{
    public class SessionInfo
    {
        public int TempCharID {get; set;}
        public int TempCampID { get; set; }
        public string TempGMPass { get; set; }
        public int SumAccount { get; set; }
        public string errorFundMessage { get; set; }

    }
 
}