using DataLayer.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer.Entities
{
    public class ConvertAddress : BaseEntity<int>
    {
        public string File1 { get; set; }

        public string File2 { get; set; }

    }
}
