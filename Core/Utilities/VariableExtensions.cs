using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Utilities
{
    public static class VariableExtensions
    {
        /// <summary>
        /// تبدیل تومان به ریال
        /// </summary>
        /// <param name="longVariable"></param>
        /// <returns></returns>
        public static long CastTomanToRial(this long longVariable)
        {
            return longVariable * 10;
        }

        /// <summary>
        /// تبدیل ریال به تومان
        /// </summary>
        /// <param name="longVariable"></param>
        /// <returns></returns>
        public static long CastRialToToman(this long longVariable)
        {
            return longVariable / 10;
        }
    }
}
