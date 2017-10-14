using System;
namespace ServiceStack.OrmLite.zly
{
    /// <summary>
    /// 标识属性为数据库中的表字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TableFieldAttribute : AttributeBase
    {
        public TableFieldAttribute() { }
    }
}
