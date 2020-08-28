using System;
using System.Collections.Generic;
using System.Text;

namespace Core.CustomAttributes
{
    public class ControllerRoleAttribute : Attribute
    {
        public string ControllerName { get; set; }

        public ControllerRoleAttribute(string controllerName)
        {
            ControllerName = controllerName;
        }

        public string GetName()
        {
            return ControllerName;
        }
    }
}
