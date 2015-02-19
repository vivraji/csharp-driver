using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Cassandra.Data.Linq;
using Cassandra.Mapping.Attributes;
using Cassandra.Mapping.Utils;

namespace Cassandra.Mapping.Config
{
    /// <summary>
    /// A config contributor that applies any type-specific ITableMapping instances registered with it
    /// to the configuration or if none is found, applies the appropriate attribute-based configuration.
    /// </summary>
    internal class ApplyMappingOrAttribtuesContributor : ITableMappingConfigContributor
    {
        private readonly LookupKeyedCollection<Type, ITableMapping> _predefinedTableMappings;
        private readonly ConcurrentDictionary<Type, Action<TableMappingConfig>> _typesUsedByLinq;

        public ApplyMappingOrAttribtuesContributor()
        {
            _predefinedTableMappings = new LookupKeyedCollection<Type, ITableMapping>(tm => tm.PocoType);
            _typesUsedByLinq = new ConcurrentDictionary<Type, Action<TableMappingConfig>>();
        }

        public void ApplyTo(TableMappingConfig tableConfig)
        {
            // Try to find a mapping and if found, apply it and bail
            ITableMapping mapping;
            if (_predefinedTableMappings.TryGetItem(tableConfig.PocoType, out mapping))
            {
                mapping.ApplyTo(tableConfig);
                return;
            }

            // See if the Type is used by LINQ and if so, apply the delegate that was registered
            Action<TableMappingConfig> applyForLinq;
            if (_typesUsedByLinq.TryGetValue(tableConfig.PocoType, out applyForLinq))
            {
                applyForLinq(tableConfig);
                return;
            }

            // Otherwise, just apply regular attribute based mappings to the configuration
            AttributeBasedTypeDefinition.ApplyTo(tableConfig);
        }
        
        /// <summary>
        /// Adds predefined table mappings to the internal cache.
        /// </summary>
        public void AddTableMappings(IEnumerable<ITableMapping> mappings)
        {
            foreach(ITableMapping mapping in mappings)
                _predefinedTableMappings.Add(mapping);
        }

        /// <summary>
        /// Tells the attribute mapping contributor that the given Type may be using the legacy LINQ API
        /// for configuration, so it should check for legacy attributes before deciding which configuration
        /// approach to use.
        /// </summary>
        [Obsolete("This can be removed once the legacy LINQ attributes are removed.")]
        public void MayBeUsingLegacyLinqApi(Type pocoType)
        {
            if (_typesUsedByLinq.ContainsKey(pocoType))
                return;

            _typesUsedByLinq.TryAdd(pocoType, CheckForLegacyLinqAttributeAndApply);
        }

        /// <summary>
        /// Tells the attribute mapping contributor that the given Type is using the legacy LINQ API with
        /// the keyspace and table names specified.  When configuration is applied, the legacy LINQ
        /// mapping configuration approach will be used.
        /// </summary>
        [Obsolete("This can be removed when the legacy LINQ attributes are removed.")]
        public void IsUsingLegacyLinqApi(Type pocoType, string keyspaceName, string tableName)
        {
            if (_typesUsedByLinq.ContainsKey(pocoType))
                return;

            _typesUsedByLinq.TryAdd(pocoType, tableConfig => LinqAttributeBasedTypeDefinition.ApplyTo(tableConfig, keyspaceName, tableName));
        }

        /// <summary>
        /// Checks for the legacy LINQ Table attribute and if found, applies those configuration changes, otherwise applies
        /// a regular attribute based mapping.
        /// </summary>
        [Obsolete("This can be removed when the legacy LINQ attributes are removed.")]
        private static void CheckForLegacyLinqAttributeAndApply(TableMappingConfig tableConfig)
        {
            // Apply the legacy configuration if the old Table attribute is present, otherwise just do a regular Attribute mapping
            if (tableConfig.PocoType.GetCustomAttributes(typeof (Cassandra.Data.Linq.TableAttribute), true).Length > 0)
            {
                LinqAttributeBasedTypeDefinition.ApplyTo(tableConfig, null, null);
                return;
            }

            AttributeBasedTypeDefinition.ApplyTo(tableConfig);
        }
    }
}