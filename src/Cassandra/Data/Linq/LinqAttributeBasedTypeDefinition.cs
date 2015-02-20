using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cassandra.Mapping;
using Cassandra.Mapping.Config;

namespace Cassandra.Data.Linq
{
    /// <summary>
    /// A type definition that uses Linq attributes on the class to determine its settings.
    /// It uses Linq default backward-compatible settings (like case sensitivity)
    /// </summary>
    [Obsolete]
    internal static class LinqAttributeBasedTypeDefinition
    {
        public static void ApplyTo(TableMappingConfig tableConfig, string keyspaceName, string tableName)
        {
            // Some legacy LINQ defaults
            tableConfig.CaseSensitive = true;
            tableConfig.ExplicitColumns = false;
            tableConfig.KeyspaceName = keyspaceName;

            if (tableName != null)
            {
                tableConfig.TableName = tableName;
            }
            else
            {
                var tableAttribute = (TableAttribute) tableConfig.PocoType.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault();
                if (tableAttribute != null)
                {
                    if (tableAttribute.Name != null)
                        tableConfig.TableName = tableAttribute.Name;

                    tableConfig.CaseSensitive = tableAttribute.CaseSensitive;
                }
            }
                
            if (tableConfig.PocoType.GetCustomAttributes(typeof(CompactStorageAttribute), true).FirstOrDefault() != null)
            {
                tableConfig.CompactStorage = true;
            }

            if (tableConfig.PocoType.GetCustomAttributes(typeof(AllowFilteringAttribute), true).FirstOrDefault() != null)
            {
                tableConfig.AllowFiltering = true;
            }

            var partitionKeys = new List<Tuple<string, int>>();
            var clusteringKeys = new List<Tuple<string, SortOrder, int>>();

            // Apply column mapping configurations before getting partition/clustering keys since those could affect column names
            foreach (IColumnMappingConfig columnConfig in tableConfig.Columns)
            {
                MemberInfo memberInfo = columnConfig.MemberInfo;

                var columnAttribute = (ColumnAttribute) memberInfo.GetCustomAttributes(typeof(ColumnAttribute), true).FirstOrDefault();
                if (columnAttribute != null)
                {
                    columnConfig.IsExplicitlyDefined = true;

                    if (columnAttribute.Name != null)
                        columnConfig.ColumnName = columnAttribute.Name;
                }

                columnConfig.SecondaryIndex = HasAttribute(memberInfo, typeof(SecondaryIndexAttribute));
                columnConfig.IsCounter = HasAttribute(memberInfo, typeof(CounterAttribute));
                columnConfig.IsStatic = HasAttribute(memberInfo, typeof(StaticColumnAttribute));
                columnConfig.Ignore = HasAttribute(memberInfo, typeof(IgnoreAttribute));

                var partitionKeyAttribute = (PartitionKeyAttribute) memberInfo.GetCustomAttributes(typeof(PartitionKeyAttribute), true).FirstOrDefault();
                if (partitionKeyAttribute != null)
                {
                    partitionKeys.Add(Tuple.Create(columnConfig.ColumnName, partitionKeyAttribute.Index));
                    continue;
                }

                var clusteringKeyAttribute = (ClusteringKeyAttribute) memberInfo.GetCustomAttributes(typeof(ClusteringKeyAttribute), true).FirstOrDefault();
                if (clusteringKeyAttribute != null)
                {
                    clusteringKeys.Add(Tuple.Create(columnConfig.ColumnName, clusteringKeyAttribute.ClusteringSortOrder, clusteringKeyAttribute.Index));
                }
            }

            // Order partition keys and clustering keys by index
            tableConfig.PartitionKeys = partitionKeys.OrderBy(k => k.Item2).Select(k => k.Item1).ToArray();
            tableConfig.ClusteringKeys = clusteringKeys.OrderBy(k => k.Item3).Select(k => Tuple.Create(k.Item1, k.Item2)).ToArray();
        }

        /// <summary>
        /// Determines if the member has an attribute applied
        /// </summary>
        private static bool HasAttribute(MemberInfo memberInfo, Type attributeType)
        {
            return memberInfo.GetCustomAttributes(attributeType, true).FirstOrDefault() != null;
        }
    }
}
