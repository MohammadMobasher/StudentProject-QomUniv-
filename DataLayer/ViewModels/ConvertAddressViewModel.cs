using DataLayer.BaseClasses;
using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer.ViewModels
{
    public class ConvertAddressViewModel : BaseMapping<ConvertAddressViewModel, ConvertAddress,int>
    {
        public string Input1 { get; set; }

        public string Input2 { get; set; }

        public string OutPut { get; set; }

        public bool IsDone { get; set; } = false;
    }
}
