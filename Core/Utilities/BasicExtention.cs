using DNTPersianUtils.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Utilities
{
    public static class BasicExtention
    {

        public static string ToPersianPrice(this long lng)
        {
            return lng.ToString("n0").ToPersianNumbers();
        }

    }
}
