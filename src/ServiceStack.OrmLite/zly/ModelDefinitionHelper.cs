using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceStack.OrmLite.zly
{
   public static class ModelDefinitionHelper
    {
       public static ModelDefinition GetModelDefinition(this Type modelType)
       {
           return OrmLiteConfigExtensions.GetModelDefinition(modelType);
       }
    }
}
