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
        [DisplayFormat(DataFormatString = "{0:n0}", ApplyFormatInEditMode = true)]
        [Display(Name = "Amount")]
        [Range(1, 999999999)]
        public int Amount { get; set; }

        [Display(Name = "Reciever")]
        public int RecId { get; set; }
        [Required]
        public string Reason { get; set; }
    }
    public class SummaryAccMetaData
    {
        [DisplayFormat(DataFormatString = "{0:n0}", ApplyFormatInEditMode = true)]
        public int Total { get; set; }
    }
    public class ModTableMetaData
    {
        [Display(Name ="Signature")]
        public string Sig { get; set; }
        [Display(Name = "Campaign")]
        public int CampID { get; set; }
        [Display(Name = "Message Header")]
        public string Heading { get; set; }
        [Display(Name = "Message Content")]
        public string Content { get; set; }
    }

}