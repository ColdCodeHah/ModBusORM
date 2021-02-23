using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ModBusORM.Extensions
{
    public static class PropertyInfoExtension
    {
        public static TAttribute GetAttribute<TAttribute>(this PropertyInfo pro)where TAttribute:Attribute
        {
            var objAttrs = pro.GetCustomAttributes(typeof(TAttribute), true);
            if (objAttrs?.Length > 0)
            {
                TAttribute attr = objAttrs[0] as TAttribute;
                return attr;
            }
            return null;
        }

    }
}
