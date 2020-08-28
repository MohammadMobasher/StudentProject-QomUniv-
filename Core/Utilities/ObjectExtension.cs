using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Utilities
{
    public static class ObjectExtension
    {
        /// <summary>
        ///  زمانی که یک ابجک داشته باشیم و مقدار یه ویژگی از آن را بخواهیم که فقط عنوان متنی آن ویژگی را داشته باشیم
        ///  از این تابع استفاده میکنیم
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        static public object GetPropertyValue(this object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName).GetValue(obj, null);
        }


        static public void SetPropertyValue(this object obj, string propertyName, object Value)
        {
            obj.GetType().GetProperty(propertyName).SetValue(obj, Value);
        }
    }
}
