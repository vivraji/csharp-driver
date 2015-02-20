using Cassandra.Mapping.Config;

namespace Cassandra.Mapping.Conventions
{
    /// <summary>
    /// A convention for mapping between tables in C* and POCOs.  When registered with a MappingConfiguration, 
    /// these will be applied to all table mapping configurations before any type-specific mappings are run.
    /// </summary>
    public interface ITableMappingConvention
    {
        /// <summary>
        /// Applies changes to the ITableMappingConfig provided that are appropriate for the convetion.
        /// </summary>
        void Apply(ITableMappingConfig config);
    }
}
