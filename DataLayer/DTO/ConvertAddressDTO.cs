using DataLayer.BaseClasses;
using DataLayer.CustomMapping;
using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer.DTO
{
    public class ConvertAddressDTO  : BaseMapping<ConvertAddressDTO, ConvertAddress,int>
    {
        public string File1 { get; set; }

        public string File2 { get; set; }
    }
}
