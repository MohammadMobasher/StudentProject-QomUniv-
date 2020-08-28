﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Utilities
{
    public static class IEnumerableExtension
    {
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> property)
        {
            return items.GroupBy(property).Select(x => x.First());
        }

        
    }
}
