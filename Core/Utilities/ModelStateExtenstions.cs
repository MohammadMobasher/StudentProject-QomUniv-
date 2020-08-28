using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Utilities
{
    public static class ModelStateExtenstions
    {
        /// <summary>
        ///  به دست آوردن تمامی اطلاعات 
        ///  Error Message 
        ///  ها در 
        ///  Model State
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public static string ExpressionsMessages(this ModelStateDictionary modelState)
        {
            var errors = modelState.Values.Where(a => a.Errors.Count > 0).Select(a => a.Errors).ToList();
            var message = default(string);

            foreach (var item in errors)
            {
                message += item.FirstOrDefault()?.ErrorMessage + ",";
            }

            return message.TrimEnd(',');
        }
    }
}
