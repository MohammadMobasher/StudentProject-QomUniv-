using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Core.Utilities
{
    public static class QueryExtension
    {

        /// <summary>
        /// درصورتی که شرط مورد نظر (پارامتر اول) برقرار باشد
        /// شرط درست اعمال میشود
        /// درغیر اینصورت شرط نادرت اعمال میشود
        /// </summary>
        /// <param name="condition">شرط تعیین کننده</param>
        /// <param name="trueExpression">شرط اعمال شونده در صورتی که پارامتر اول درست باشد</param>
        /// <param name="falseExpression">شرط اعمال شونده در صورتی که پارامتر اول درست نباشد</param>
        /// <returns></returns>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> source,
            bool condition,
            Expression<Func<T, bool>> trueExpression,
            Expression<Func<T, bool>> falseExpression = null)
        {



            //درصورتی که شرط مورد نظر برقرار بود
            if (condition)
                source = source.Where(trueExpression);
            //درغیر اینصورت
            else
            {
                if (falseExpression != null)
                    source = source.Where(falseExpression);
            }

            return source;

        }


        /// <summary>
        /// درصورتی که شرط مورد نظر (پارامتر اول) برقرار باشد
        /// مرتب سازی بر آن اساس اعمال می گردد
        /// </summary>
        /// <param name="condition">شرط تعیین کننده</param>
        /// <param name="oderByException">بر اساس چه فیلدی مرتب سازی گردد</param>
        /// <returns></returns>
        public static IQueryable<T> OrderByIf<T>(this IQueryable<T> source,
            bool condition,
            Func<IQueryable<T>, IOrderedQueryable<T>> oderByException)
        {
            //درصورتی که شرط مورد نظر برقرار بود
            if (condition)
                source = oderByException(source);

            return source;

        }
    }
}
