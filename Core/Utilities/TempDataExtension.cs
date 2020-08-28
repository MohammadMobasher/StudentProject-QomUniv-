using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Utilities
{
    public static class TempDataExtension
    {
        public static void AddResult(this ITempDataDictionary tempData, SweetAlertExtenstion result)
        {
            tempData["ServiceResult.Succeed"] = result.Succeed;
            tempData["ServiceResult.Message"] = result.Message;
        }
    }
}
