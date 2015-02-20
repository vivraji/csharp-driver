using System;
using System.Collections.Generic;
using Cassandra.Mapping.Conventions;

namespace Cassandra.Mapping.Config
{
    /// <summary>
    /// Contributor that applies conventions to the table config.  Conventions can be added by using the AddConvention method
    /// and are applied in the order they are added.
    /// </summary>
    internal class ApplyConventionsContributor : ITableMappingConfigContributor
    {
        private readonly List<ITableMappingConvention> _conventions;
 
        public ApplyConventionsContributor()
        {
            _conventions = new List<ITableMappingConvention>();
        }

        public void ApplyTo(TableMappingConfig tableConfig)
        {
            // Apply each convention in the order they were added
            foreach (ITableMappingConvention convention in _conventions)
            {
                convention.Apply(tableConfig);
            }
        }

        /// <summary>
        /// Adds a convention to the internal cache of conventions.  Conventions are run in the order they are added.
        /// </summary>
        public void AddConvention(ITableMappingConvention convention)
        {
            if (convention == null)
            {
                throw new ArgumentNullException("convention");
            }
            _conventions.Add(convention);
        }
    }
}
