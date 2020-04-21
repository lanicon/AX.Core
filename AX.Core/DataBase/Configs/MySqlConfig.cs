﻿using System;
using System.Reflection;
using System.Text;

namespace AX.Core.DataBase.Configs
{
    public class MySqlConfig : IDBDialectConfig
    {
        public string LeftEscapeChar { get { return "`"; } }

        public string RightEscapeChar { get { return "`"; } }

        public string DbParmChar { get { return "@"; } }

        public string GetTableExitSql(string tableName, string dataBaseName)
        {
            return $"SELECT COUNT(*) FROM information_schema.`TABLES` WHERE TABLE_NAME = '{tableName}' AND TABLE_SCHEMA = '{dataBaseName}'";
        }

        public string GetFiledExitSql(string fieldName, string tableName, string dataBaseName)
        {
            return $"SELECT COUNT(*) FROM information_schema.`COLUMNS` WHERE TABLE_NAME = '{tableName}' AND COLUMN_NAME = '{fieldName}' AND TABLE_SCHEMA = '{dataBaseName}'";
        }

        public string GetCreateTableSql(string tableName, string KeyName, PropertyInfo[] propertyInfos)
        {
            var result = new StringBuilder();

            result.Append($"CREATE TABLE IF NOT EXISTS {tableName} (");
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                var item = propertyInfos[i];
                result.Append($"{item.Name.ToLower()} {GetType(item)}");
                if (i != propertyInfos.Length)
                { result.Append($","); }
            }
            result.Append($"PRIMARY KEY({KeyName})");
            result.Append($")");
            result.Append($"ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COMMENT = '{tableName}';");

            return result.ToString();
        }

        public string GetCreateFieldSql(string tableName, PropertyInfo item)
        {
            return $"ALTER TABLE {tableName} ADD COLUMN {item.Name.ToLower()} {GetType(item)} DEFAULT NULL;";
        }

        private string GetType(PropertyInfo item)
        {
            //数值类
            if (item.PropertyType.FullName == typeof(int).FullName)
            { return "int(11)"; }
            if (item.PropertyType.FullName == typeof(double).FullName)
            { return "double"; }
            if (item.PropertyType.FullName == typeof(bool).FullName)
            { return "bit(1)"; }
            if (item.PropertyType.FullName == typeof(decimal).FullName)
            { return "decimal(10, 2)"; }
            if (item.PropertyType.FullName == typeof(decimal?).FullName)
            { return "decimal(10, 2)"; }

            //时间
            if (item.PropertyType.FullName == typeof(DateTime).FullName)
            { return "datetime"; }
            if (item.PropertyType.FullName == typeof(DateTime?).FullName)
            { return "datetime"; }

            //字符串
            if (item.PropertyType.FullName == typeof(string).FullName)
            { return "varchar(255)"; }

            return "未匹配类型";
        }
    }
}