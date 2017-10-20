using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceStack.OrmLite.zly
{
    public class Interceptor
    {
        public static Func<object, string, object[], object> Invoker;
        public object Invoke(object @object, string @method, object[] parameters)
        {
            if (Invoker != null)
                return Invoker(@object, @method, parameters);
            else return @object.GetType().GetMethod(@method).Invoke(@object, parameters);

            //Console.WriteLine(
            //  string.Format("Interceptor does something before invoke [{0}]...", @method));

            //var retObj = @object.GetType().GetMethod(@method).Invoke(@object, parameters);

            //Console.WriteLine(
            //  string.Format("Interceptor does something after invoke [{0}]...", @method));

            //return retObj;
        }
    }
}
