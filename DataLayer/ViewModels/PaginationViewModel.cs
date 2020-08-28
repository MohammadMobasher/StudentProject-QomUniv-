using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer.ViewModels
{
    public class PaginationViewModel
    {
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
