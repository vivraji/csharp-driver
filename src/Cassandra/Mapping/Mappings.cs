using System;
using Cassandra.Mapping.Utils;

namespace Cassandra.Mapping
{
    /// <summary>
    /// A class for defining how to map multiple POCOs via a fluent-style interface.  Inheritors should use the 
    /// <see cref="For{TPoco}"/> method inside their constructor to define mappings.
    /// </summary>
    public abstract class Mappings
    {
        internal LookupKeyedCollection<Type, ITableMapping> TableMappings;

        /// <summary>
        /// Creates a new collection of mappings.  Inheritors should define all their mappings in the constructor of the sub-class.
        /// </summary>
        protected Mappings()
        {
            TableMappings = new LookupKeyedCollection<Type, ITableMapping>(td => td.PocoType);
        }

        /// <summary>
        /// Adds a mapping for the Poco type specified (TPoco).
        /// </summary>
        public Map<TPoco> For<TPoco>()
        {
            ITableMapping map;
            if (TableMappings.TryGetItem(typeof (TPoco), out map) == false)
            {
                map = new Map<TPoco>();
                TableMappings.Add(map);
            }

            return (Map<TPoco>) map;
        }
    }
}