using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cassandra.Mapping.Config;

namespace Cassandra.Mapping.Attributes
{
    /// <summary>
    /// A type definition that uses attributes on the class to determine its settings.
    /// </summary>
    internal static class AttributeBasedTypeDefinition
    {
        public static void ApplyTo(TableMappingConfig tableConfig)
        {
            tableConfig.TableName = tableConfig.PocoType.Name;

            //Get the table name from the attribute or the type name
            var tableAttribute = (TableAttribute) tableConfig.PocoType.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault();
            if (tableAttribute != null)
            {
                tableConfig.TableName = tableAttribute.Name;
                tableConfig.KeyspaceName = tableAttribute.Keyspace;
                tableConfig.CaseSensitive = tableAttribute.CaseSensitive;
                tableConfig.CompactStorage = tableAttribute.CompactStorage;
                tableConfig.AllowFiltering = tableAttribute.AllowFiltering;
                tableConfig.ExplicitColumns = tableAttribute.ExplicitColumns;
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

                    if (columnAttribute.Type != null)
                        columnConfig.ColumnType = columnAttribute.Type;
                }

                columnConfig.Ignore = HasAttribute(memberInfo, typeof(IgnoreAttribute));
                columnConfig.SecondaryIndex = HasAttribute(memberInfo, typeof(SecondaryIndexAttribute));
                columnConfig.IsStatic = HasAttribute(memberInfo, typeof(StaticColumnAttribute));
                columnConfig.IsCounter = HasAttribute(memberInfo, typeof(CounterAttribute));

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