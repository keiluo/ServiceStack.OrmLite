using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceStack.OrmLite.zly
{
    /// <summary>
    /// 自定义特性 类可用  支持继承
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class DbLinkNameAttribute : Attribute
    {
        public DbLinkNameAttribute(string name)
        {
            Name = name;
        }
        private string _Name;
        /// <summary>
        /// 配置文件config/db.config中的name
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

    }
}
