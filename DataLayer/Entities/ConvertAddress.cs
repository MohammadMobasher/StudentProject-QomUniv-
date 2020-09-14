using DataLayer.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer.Entities
{
    public class ConvertAddress : BaseEntity<int>
    {
        public string Input1 { get; set; }

        public string Input2 { get; set; }

        public string OutPut { get; set; }

        public bool IsDone { get; set; } = false;



    }
}
