using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cassandra.Mapping.Attributes;
using Cassandra.Mapping.Utils;

namespace Cassandra.Mapping
{
    /// <summary>
    /// Factory responsible for creating PocoData instances, uses AttributeBasedTypeDefinition to create new Poco information in case a definition was not provided.
    /// </summary>
    internal class PocoDataFactory
    {
        private const BindingFlags PublicInstanceBindingFlags = BindingFlags.Public | BindingFlags.Instance;

        private readonly LookupKeyedCollection<Type, ITableMapping> _predefinedTableMappings;
        private readonly ConcurrentDictionary<Type, PocoData> _cache;

        /// <summary>
        /// Creates a new factory responsible of PocoData instances.
        /// </summary>
        /// <param name="predefinedTableMappings">Explicitly declared table mappings.</param>
        public PocoDataFactory(LookupKeyedCollection<Type, ITableMapping> predefinedTableMappings)
        {
            if (predefinedTableMappings == null) throw new ArgumentNullException("predefinedTableMappings");
            _predefinedTableMappings = predefinedTableMappings;
            _cache = new ConcurrentDictionary<Type, PocoData>();
        }

        public PocoData GetPocoData<T>()
        {
            return _cache.GetOrAdd(typeof(T), CreatePocoData);
        }

        /// <summary>
        /// Adds a definition to the local state in case no definition was explicitly defined.
        /// Used when the local default (AttributeBasedTypeDefinition) is not valid for a given type.
        /// </summary>
        public void AddDefinitionDefault(Type type, Func<ITableMapping> definitionHandler)
        {
            //In case there isn't already Poco information in the local cache.
            if (_predefinedTableMappings.Contains(type))
            {
                return;
            }
            _cache.GetOrAdd(type, t => CreatePocoData(t, definitionHandler()));
        }
        
        private PocoData CreatePocoData(Type pocoType)
        {
            // Try to get mapping from predefined collection, otherwise fallback to using attributes
            ITableMapping typeMapping;
            if (_predefinedTableMappings.TryGetItem(pocoType, out typeMapping) == false)
            {
                typeMapping = new AttributeBasedTypeDefinition(pocoType);
            }
            return CreatePocoData(pocoType, typeMapping);
        }

        private PocoData CreatePocoData(Type pocoType, ITableMapping typeMapping)
        {
            // Create config and allow mapping to modify it
            var tableConfig = new TableMappingConfig(pocoType);
            typeMapping.ApplyTo(tableConfig);

            // Figure out the table name (if not specified, use the POCO class' name)
            string tableName = tableConfig.TableName ?? pocoType.Name;

            // Figure out the primary key columns (if not specified, assume a column called "id" is used)
            var pkColumnNames = tableConfig.PartitionKeys ?? new[] { "id" };

            // Create PocoColumn collection (where ordering is guaranteed to be consistent)
            Func<ColumnMappingConfig, bool> shouldIncludeColumn = tableConfig.ExplicitColumns
                                                                      ? (Func<ColumnMappingConfig, bool>) (c => c.IsExplicitlyDefined)
                                                                      : (c => c.Ignore == false);

            LookupKeyedCollection<string, PocoColumn> columns = tableConfig.Columns.Where(shouldIncludeColumn)
                                                                           .Select(PocoColumn.FromColumnMappingConfig)
                                                                           .ToLookupKeyedCollection(pc => pc.ColumnName,
                                                                                                    StringComparer.OrdinalIgnoreCase);
            
            var clusteringKeyNames = tableConfig.ClusteringKeys ?? new Tuple<string, SortOrder>[0];
            return new PocoData(pocoType, tableName, tableConfig.KeyspaceName, columns, pkColumnNames, clusteringKeyNames, tableConfig.CaseSensitive,
                                tableConfig.CompactStorage, tableConfig.AllowFiltering);
        }

        /// <summary>
        /// Gets any public instance fields that are settable for the given type.
        /// </summary>
        internal static IEnumerable<FieldInfo> GetMappableFields(Type t)
        {
            return t.GetFields(PublicInstanceBindingFlags).Where(field => field.IsInitOnly == false);
        }

        /// <summary>
        /// Gets any public instance properties for the given type.
        /// </summary>
        private static IEnumerable<PropertyInfo> GetMappableProperties(Type t)
        {
            return t.GetProperties(PublicInstanceBindingFlags).Where(p => p.CanWrite);
        }
    }
}