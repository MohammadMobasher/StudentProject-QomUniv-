using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer.ViewModels
{
    public class IdTitle<T> 
    {
        public T Id { get; set; }

        public string Title { get; set; }
    }

    public class IdTitle
    {
        public int Id { get; set; }

        public string Title { get; set; }
    }


    public class select2IdText
    {
        public int Id { get; set; }

        public string Text { get; set; }
    }

    public class select2IdTextImage
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public string Image { get; set; }
    }


}
