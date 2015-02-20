using System;
using Cassandra.Mapping.Config;

namespace Cassandra.Mapping.Conventions
{
    /// <summary>
    /// A mapping convention that will apply the columnNameConverter function specified in the constructor to
    /// all columns of a table.  Static properties contain some built-in naming conventions like converting
    /// column names to upper/lower case, or converting from CamelCase to underscores.
    /// </summary>
    public class ColumnNamingConvention : ITableMappingConvention
    {
        /// <summary>
        /// Naming convention for converting all table names to upper case.  For example: "UsersTable" -> "USERSTABLE".
        /// </summary>
        public static readonly ITableMappingConvention ToUpperCase = new ColumnNamingConvention(NameConverters.ToUpper);

        /// <summary>
        /// Naming convention for converting all table names to lower case.  For example: "UsersTable" -> "userstable".
        /// </summary>
        public static readonly ITableMappingConvention ToLowerCase = new ColumnNamingConvention(NameConverters.ToLower);

        /// <summary>
        /// Naming convention for converting CamelCase table names to ones with underscores.  For example: "UsersTable" -> "Users_Table",
        /// "JobNotifications" -> "Job_Notifications", etc. 
        /// </summary>
        public static readonly ITableMappingConvention CamelCaseToUnderscore = new ColumnNamingConvention(NameConverters.CamelCaseToUnderscore);

        private readonly Func<string, string> _columnNameConverter;

        /// <summary>
        /// Creates a new column naming convention that will apply the specified converter Func to all column names of a table.
        /// </summary>
        /// <param name="columnNameConverter"></param>
        public ColumnNamingConvention(Func<string, string> columnNameConverter)
        {
            if (columnNameConverter == null)
            {
                throw new ArgumentNullException("columnNameConverter");
            }
            _columnNameConverter = columnNameConverter;
        }

        /// <summary>
        /// Applies the naming convention to all columns in a table.
        /// </summary>
        public void Apply(ITableMappingConfig config)
        {
            foreach (IColumnMappingConfig columnConfig in config.Columns)
            {
                columnConfig.ColumnName = _columnNameConverter(columnConfig.ColumnName);
            }
        }
    }
}