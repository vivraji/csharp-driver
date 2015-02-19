using System;
using Cassandra.Mapping.Config;

namespace Cassandra.Mapping
{
    /// <summary>
    /// Component capable of configuring the mapping between a particular POCO/Type and Cassandra.
    /// </summary>
    public interface ITableMapping
    {
        /// <summary>
        /// The .NET Type this mapping applies to.
        /// </summary>
        Type PocoType { get; }

        /// <summary>
        /// Modifies the configuration object provided to configure the mapping between Cassandra
        /// and the .NET Type specified by <see cref="PocoType"/>.
        /// </summary>
        void ApplyTo(ITableMappingConfig tableConfig);
    }
}
