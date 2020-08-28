using Core.CustomAttributes;
using Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Service.Repos.Basic
{
    public class BasicRepository
    {

        public Dictionary<string, List<MethodInfo>> GetControllerAndActionByCustomAttribute(List<Type> types)
        {
            var lst = new Dictionary<string, List<MethodInfo>>();

            foreach (var item in types)
            {
                var actions = item.GetMethods().Where(a => a.HasAttribute(typeof(ActionRoleAttribute))).ToList();
                if (actions == null || actions.Count == 0) continue;

                var controllerName = actions.FirstOrDefault().ReflectedType.Name;

                lst.Add(controllerName, actions);
            }

            return lst;
        }

        /// <summary>
        /// گرفتن اطلاعات تمامی کنترلر هایی که از 
        /// ControllerRoleAttribute
        /// استفاده میکنند
        /// </summary>
        /// <returns></returns>
        public List<Type> GetAllControllerUseControllerRoleAttribute(params Assembly[] assemblies)
            =>  typeof(ControllerRoleAttribute).GetTypesHasAttribute(assemblies).ToList();

        
    }
}
