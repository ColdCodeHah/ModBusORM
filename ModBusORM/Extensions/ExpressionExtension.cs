using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ModBusORM.Extensions
{
    public static class ExpressionExtension
    {
        public static PropertyInfo GetProperty<T,TResult>(this Expression<Func<T, TResult>> express)
        {
            if(express is LambdaExpression)
            {
                MemberExpression body = (MemberExpression)express.Body;
                var pro = typeof(T).GetProperty(body.Member.Name);
                return pro;
            }
            return null;
        }       

        public static Dictionary<string,object> GetPropertyNameAndValue<T>(this Expression<Func<T, T>> express)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if(express is LambdaExpression)
            {
                MemberInitExpression body = (MemberInitExpression)express.Body;
                foreach (MemberAssignment item in body.Bindings)
                {
                    var name = item.Member.Name;                    
                    var single = item.Expression;                               
                    //var value = single.Value;
                    //dic[name] = value;
                }
            }
            return dic;
        }


    }
}
