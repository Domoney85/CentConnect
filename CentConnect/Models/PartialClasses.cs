using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CentConnect.Models
{
    [MetadataType(typeof(CharAccMetaData))]
    public partial class CharAcc
    {

    }
    [MetadataType(typeof(TransactionMetaData))]
    public partial class Transaction
    {

    }
    [MetadataType(typeof(SummaryAccMetaData))]
    public partial class SummaryAcc
    {

    }
    [MetadataType(typeof(ModTableMetaData))]
    public partial class ModTable
    {

    }
}