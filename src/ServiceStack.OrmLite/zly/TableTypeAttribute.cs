using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceStack.OrmLite.zly
{
    public enum TableTypeEnum { TableView, Sql }
    /// <summary>
    /// 标识属性为数据库中的表字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableTypeAttribute : AttributeBase
    {
        public TableTypeEnum TableTypeValue { get; set; }
        public TableTypeAttribute(TableTypeEnum tableTypeValue = TableTypeEnum.TableView)
        {
            this.TableTypeValue = tableTypeValue;
        }
    }
}
