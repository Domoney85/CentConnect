using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CentConnect.Models
{
    public class TransPackage
    {
        [Display(Name = "Character ID")]
        public int tName=0;
        public string TransTime;

        public string recieve;
 
        public string send;
        public string reason;

        public TransPackage(int n, int r,int s, string re, DateTime? Tr)
        {
            tName = n;
            if (r > 0) recieve = String.Format("{0:n0}", r)+" Cr"; else recieve = null;
            if (s > 0) send = String.Format("{0:n0}", s) + " Cr";  else send = null;
            reason = re.ToString();
            TransTime = Tr.ToString();

        }
    }
}