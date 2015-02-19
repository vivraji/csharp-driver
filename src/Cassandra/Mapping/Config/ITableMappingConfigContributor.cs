namespace Cassandra.Mapping.Config
{
    /// <summary>
    /// A component that contributes to configuring table mapping between C* and POCOs.
    /// </summary>
    internal interface ITableMappingConfigContributor
    {
        /// <summary>
        /// Modifies the configuration as appropriate.
        /// </summary>
        void ApplyTo(TableMappingConfig tableConfig);
    }
}
