using System;
using System.Collections.Generic;
using System.Text;

namespace Core.CustomAttributes
{
    public class ActionRoleAttribute : Attribute
    {
        public string ActionName { get; set; }

        public ActionRoleAttribute(string actionName)
        {
            ActionName = actionName;
        }

        public string GetActionName()
        {
            return ActionName;
        }
    }
}
