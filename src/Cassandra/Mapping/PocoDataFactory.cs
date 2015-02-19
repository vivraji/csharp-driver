using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Cassandra.Mapping.Config;
using Cassandra.Mapping.Utils;

namespace Cassandra.Mapping
{
    /// <summary>
    /// Factory responsible for creating PocoData instances, uses AttributeBasedTypeDefinition to create new Poco information in case a definition was not provided.
    /// </summary>
    internal class PocoDataFactory
    {
        private readonly IList<ITableMappingConfigContributor> _configPipeline;
        private readonly ConcurrentDictionary<Type, PocoData> _cache;

        /// <summary>
        /// Creates a new factory responsible of PocoData instances.
        /// </summary>
        /// <param name="configPipeline">A list of configuration contributors to run when creating PocoData classes.</param>
        public PocoDataFactory(IList<ITableMappingConfigContributor> configPipeline)
        {
            if (configPipeline == null)
            {
                throw new ArgumentNullException("configPipeline");
            }
            _configPipeline = configPipeline;

            _cache = new ConcurrentDictionary<Type, PocoData>();
        }

        public PocoData GetPocoData<T>()
        {
            return _cache.GetOrAdd(typeof(T), CreatePocoData);
        }
        
        private PocoData CreatePocoData(Type pocoType)
        {
            // Create config and allow config pipeline to modify it
            var tableConfig = new TableMappingConfig(pocoType);
            foreach(ITableMappingConfigContributor contributor in _configPipeline)
                contributor.ApplyTo(tableConfig);
            
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
    }
}