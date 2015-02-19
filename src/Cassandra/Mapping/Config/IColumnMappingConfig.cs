using System;
using System.Reflection;

namespace Cassandra.Mapping.Config
{
    /// <summary>
    /// The configuration for how to map a property/field on a POCO to a column in Cassandra.  Can be
    /// inspected or modified.
    /// </summary>
    public interface IColumnMappingConfig
    {
        /// <summary>
        /// Gets the .NET Type of the POCO the column being configured belongs to.
        /// </summary>
        Type PocoType { get; }

        /// <summary>
        /// Gets the MemberInfo for the property or field being configured.
        /// </summary>
        MemberInfo MemberInfo { get; }

        /// <summary>
        /// Gets the return Type of the property or field being configured (i.e. FieldInfo.FieldType or PropertyInfo.PropertyType).
        /// </summary>
        Type MemberInfoType { get; }

        /// <summary>
        /// Gets or sets the name of the column in the database that this property/field maps to.
        /// </summary>
        string ColumnName { get; set; }

        /// <summary>
        /// Gets or sets the data type of the column in C* for inserting/updating data.
        /// </summary>
        Type ColumnType { get; set; }

        /// <summary>
        /// Gets or sets whether the property/field should be ignored when mapping.
        /// </summary>
        bool Ignore { get; set; }

        /// <summary>
        /// Gets or sets whether this column has been explicitly defined (for use when ITypeDefinitionConfig.ExplicitColumns is true).
        /// </summary>
        bool IsExplicitlyDefined { get; set; }

        /// <summary>
        /// Gets or sets whether there is a secondary index defined for this column.
        /// </summary>
        bool SecondaryIndex { get; set; }

        /// <summary>
        /// Gets or sets whether this column is a counter column.
        /// </summary>
        bool IsCounter { get; set; }

        /// <summary>
        /// Gets or sets whether this column is a static column.
        /// </summary>
        bool IsStatic { get; set; }
    }
}