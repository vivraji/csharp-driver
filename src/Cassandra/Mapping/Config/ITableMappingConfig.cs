using System;
using System.Collections.Generic;

namespace Cassandra.Mapping.Config
{
    /// <summary>
    /// The configuration for how to map a POCO to a table in Cassandra.  Can be inspected or modified.
    /// </summary>
    public interface ITableMappingConfig
    {
        /// <summary>
        /// Gets the .NET Type of the POCO that is configured by this class.
        /// </summary>
        Type PocoType { get; }

        /// <summary>
        /// Gets or sets the table name that the POCO maps to.
        /// </summary>
        string TableName { get; set; }

        /// <summary>
        /// Gets or sets the name of the keyspace where the table is defined.  When not null, the table name for the 
        /// query generated will be fully qualified (ie: keyspace.tablename).
        /// </summary>
        string KeyspaceName { get; set; }

        /// <summary>
        /// Gets or sets whether or not this POCO should only have columns explicitly defined mapped.
        /// </summary>
        bool ExplicitColumns { get; set; }

        /// <summary>
        /// Gets or sets the partition key columns of the table.
        /// </summary>
        IList<string> PartitionKeys { get; set; }

        /// <summary>
        /// Gets or sets the clustering key columns of the table.
        /// </summary>
        IList<Tuple<string, SortOrder>> ClusteringKeys { get; set; }

        /// <summary>
        /// Determines if the queries generated for this POCO should be case-sensitive
        /// </summary>
        bool CaseSensitive { get; set; }

        /// <summary>
        /// Gets or sets whether the table for the POCO is declared with COMPACT STORAGE
        /// </summary>
        bool CompactStorage { get; set; }

        // TODO: Only expose this internally for backwards compatibility
        bool AllowFiltering { get; set; }

        /// <summary>
        /// A read-only collection of column configurations for this POCO.  The individual column configurations
        /// can be inspected/modified, but the collection itself cannot be changed.
        /// </summary>
        IEnumerable<IColumnMappingConfig> Columns { get; }
    }

}
