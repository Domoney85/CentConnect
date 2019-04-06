using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CentConnect.Models
{
    public class CharAccMetaData
    {

        [Display(Name ="Character ID")]
        public int CharId { get; set; }
        [Display(Name = "Character Name")]
        public string CharName { get; set; }
        [Display(Name = "Is Char Alive?")]
        public Nullable<bool> IsAlive { get; set; }
        [Display(Name = "GM Password")]
        public string IsGM { get; set; }
        [Display(Name = "Campaign ID")]
        public int CampID { get; set; }
        [Display(Name = "Account ID")]
        public string AccId { get; set; }
       
    }
    public class TransactionMetaData
    {

        [Display(Name = "Amount")]
        [Range(0, 999999999)]
        public int Amount { get; set; }
    }
}