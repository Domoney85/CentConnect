using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CentConnect.Models
{
    public class TransPackage
    {
        public string tName="Unknown";
        public string TransTime;
        public string recieve;
        public string send;
        public string reason;

        public TransPackage(string n, int r,int s, string re, DateTime? Tr)
        {
            tName = n;
            if (r > 0) recieve = r.ToString(); else recieve = null;
            if (s > 0) send = s.ToString(); else send = null;
            reason = re.ToString();
            TransTime = Tr.ToString();

        }
    }
}