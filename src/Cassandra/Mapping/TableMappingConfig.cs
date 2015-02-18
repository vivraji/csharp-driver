using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cassandra.Mapping
{
    /// <summary>
    /// Represents mutable configuration about how to map a POCO/Type to a table in C*.
    /// </summary>
    internal class TableMappingConfig : ITableMappingConfig
    {
        private const BindingFlags PublicInstanceBindingFlags = BindingFlags.Public | BindingFlags.Instance;

        private readonly Type _pocoType;
        private readonly List<ColumnMappingConfig> _columns;
        private string _tableName;
        private IList<string> _partitionKeys;
        private IList<Tuple<string, SortOrder>> _clusteringKeys; 

        public Type PocoType
        {
            get { return _pocoType; }
        }

        public string TableName
        {
            get { return _tableName; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentOutOfRangeException("value", "TableName cannot be set to null or whitespace.");

                _tableName = value;
            }
        }

        public string KeyspaceName { get; set; }
        
        public bool ExplicitColumns { get; set; }

        public IList<string> PartitionKeys
        {
            get { return _partitionKeys; }
            set
            {
                if (value == null) throw new ArgumentNullException("value", "PartitionKeys cannot be set to null.");
                _partitionKeys = value;
            }
        }

        public IList<Tuple<string, SortOrder>> ClusteringKeys
        {
            get { return _clusteringKeys; }
            set
            {
                if (value == null) throw new ArgumentNullException("value", "ClusteringKeys cannot be set to null.");
                _clusteringKeys = value;
            }
        }

        public bool CaseSensitive { get; set; }

        public bool CompactStorage { get; set; }

        // TODO: Exposed internally only?
        public bool AllowFiltering { get; set; }

        IEnumerable<IColumnMappingConfig> ITableMappingConfig.Columns
        {
            get { return _columns; }
        }

        public List<ColumnMappingConfig> Columns 
        {
            get { return _columns; }
        }
        
        public TableMappingConfig(Type pocoType)
        {
            if (pocoType == null) throw new ArgumentNullException("pocoType");
            _pocoType = pocoType;

            // Default assumptions
            TableName = pocoType.Name;
            KeyspaceName = null;
            ExplicitColumns = false;
            _partitionKeys = new List<string> { "id" };
            _clusteringKeys = new List<Tuple<string, SortOrder>>();
            CaseSensitive = false;
            CompactStorage = false;
            AllowFiltering = false;

            // Create some default column definitions for all mappable fields/properties
            _columns = pocoType.GetFields(PublicInstanceBindingFlags)
                               .Where(fi => fi.IsInitOnly == false)
                               .Select(fi => new ColumnMappingConfig(pocoType, fi))
                               .Union(
                                   pocoType.GetProperties(PublicInstanceBindingFlags)
                                           .Where(pi => pi.CanWrite)
                                           .Select(pi => new ColumnMappingConfig(pocoType, pi)))
                               .ToList();
        }
    }
}