﻿using AX.Core.Cache;
using AX.Core.CommonModel.Exceptions;
using AX.Core.DataBase.Schema.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace AX.Core.DataBase.Schema
{
    public static class SchemaProvider
    {
        private static readonly MemoryCache<string> _tableNameCache;

        static SchemaProvider()
        {
            _tableNameCache = CacheFactory.CreateCache<string>("数据实体类表名缓存") as MemoryCache<string>;
        }

        public static string GetTableName<T>()
        {
            var result = string.Empty;
            var typeFullName = typeof(T).FullName;

            if (_tableNameCache.ContainsKey(typeFullName))
            { return _tableNameCache[typeFullName]; }
            else
            {
                var tableattr = typeof(T)
.GetCustomAttributes(true)
.SingleOrDefault(attr => attr.GetType().Name == typeof(TableAttribute).Name) as TableAttribute;

                if (tableattr != null)
                { _tableNameCache[typeFullName] = tableattr.Name; }
                else
                { _tableNameCache[typeFullName] = typeof(T).Name; }

                return _tableNameCache[typeFullName];
            }
        }

        public static PropertyInfo GetPrimaryKey<T>()
        {
            PropertyInfo prop = null;
            var allProperties = typeof(T).GetProperties().ToList();

            var primaryKey = allProperties.SingleOrDefault(p => p.GetCustomAttribute(typeof(KeyAttribute)) != null);
            if (primaryKey != null)
            { return primaryKey; }

            //若没有特性标注 则默认寻找Id名称属性 默认不自增
            prop = allProperties.FirstOrDefault(p => p.Name.Equals("Id"));
            if (prop != null)
            { return prop; }

            throw new AXDataBaseException($"【{typeof(T).FullName}】 未找到主键标注", string.Empty);
        }

        public static List<PropertyInfo> GetSchemaProperties<T>()
        {
            var allProperties = typeof(T).GetProperties();
            var result = new List<PropertyInfo>();
            for (int i = 0; i < allProperties.Length; i++)
            {
                var prop = allProperties[i];
                var propAttributes = prop.GetCustomAttributes(true);
                if (propAttributes.Any(s => s.GetType() == typeof(IgnoreAttribute)))
                { continue; }
                result.Add(prop);
            }
            return result;
        }

        public static List<PropertyInfo> GetInsertProperties<T>()
        {
            var allProperties = typeof(T).GetProperties();
            var result = new List<PropertyInfo>();
            for (int i = 0; i < allProperties.Length; i++)
            {
                var prop = allProperties[i];
                var propAttributes = prop.GetCustomAttributes(true);
                if (propAttributes.Any(s => s.GetType() == typeof(IgnoreAttribute)))
                { continue; }
                if (propAttributes.Any(s => s.GetType() == typeof(OnlySelectAttribute)))
                { continue; }
                result.Add(prop);
            }
            return result;
        }

        public static List<PropertyInfo> GetSelectProperties<T>()
        {
            var allProperties = typeof(T).GetProperties();
            var result = new List<PropertyInfo>();
            for (int i = 0; i < allProperties.Length; i++)
            {
                var prop = allProperties[i];
                var propAttributes = prop.GetCustomAttributes(true);
                if (propAttributes.Any(s => s.GetType() == typeof(IgnoreAttribute)))
                { continue; }
                result.Add(prop);
            }
            return result;
        }

        public static List<PropertyInfo> GetUpdataProperties<T>()
        {
            var allProperties = typeof(T).GetProperties();
            var result = new List<PropertyInfo>();
            for (int i = 0; i < allProperties.Length; i++)
            {
                var prop = allProperties[i];
                var propAttributes = prop.GetCustomAttributes(true);
                if (propAttributes.Any(s => s.GetType() == typeof(IgnoreAttribute)))
                { continue; }
                if (propAttributes.Any(s => s.GetType() == typeof(OnlySelectAttribute)))
                { continue; }
                result.Add(prop);
            }
            return result;
        }
    }
}