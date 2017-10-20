using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceStack.OrmLite.zly
{
    /// <summary>
    /// 标识属性为引用类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TableReferenceAttribute : AttributeBase
    {
        public string SourceField { get; set; }
        public Type TargetClass { get; set; }
        public string TargetField { get; set; }
        /// <summary>
        /// 是否级联更新，即当SourceField的值改变时会更新到TargetClass的TargetField
        /// </summary>
        public bool IsCascade { get; set; }
        public TableReferenceAttribute(string sourceField, Type targetClass, string targetField)
        {
            SourceField = sourceField;
            TargetClass = targetClass;
            TargetField = targetField;
        }
    }
}
