using System;
using System.Collections.Generic;
using System.Text;

namespace Core.CustomAttributes
{
    public class ColumnGridViewAttribute : Attribute
    {

        public string Width { get; set; }

        public bool Searchable { get; set; }

        public bool Sortable { get; set; }

        public string DisplayName { get; set; }

        public bool AlignCenter { get; set; } = false;



    }
}
