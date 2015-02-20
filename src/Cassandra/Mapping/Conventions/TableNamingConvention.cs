using System;
using Cassandra.Mapping.Config;

namespace Cassandra.Mapping.Conventions
{
    /// <summary>
    /// A mapping convention that will apply the tableNameConverter function specified in the constructor to
    /// all table names.  Static properties contain some built-in naming conventions like converting table names
    /// to all upper/lower case, or converting from CamelCase to underscores.
    /// </summary>
    public class TableNamingConvention : ITableMappingConvention
    {
        /// <summary>
        /// Naming convention for converting all table names to upper case.  For example: "UsersTable" -> "USERSTABLE".
        /// </summary>
        public static readonly ITableMappingConvention ToUpperCase = new TableNamingConvention(NameConverters.ToUpper);

        /// <summary>
        /// Naming convention for converting all table names to lower case.  For example: "UsersTable" -> "userstable".
        /// </summary>
        public static readonly ITableMappingConvention ToLowerCase = new TableNamingConvention(NameConverters.ToLower);

        /// <summary>
        /// Naming convention for converting CamelCase table names to ones with underscores.  For example: "UsersTable" -> "Users_Table",
        /// "JobNotifications" -> "Job_Notifications", etc. 
        /// </summary>
        public static readonly ITableMappingConvention CamelCaseToUnderscore = new TableNamingConvention(NameConverters.CamelCaseToUnderscore);

        private readonly Func<string, string> _tableNameConverter;

        /// <summary>
        /// Creates a new table naming convention that will convert table names using the Func specified.
        /// </summary>
        public TableNamingConvention(Func<string, string> tableNameConverter)
        {
            if (tableNameConverter == null)
            {
                throw new ArgumentNullException("tableNameConverter");
            }
            _tableNameConverter = tableNameConverter;
        }

        /// <summary>
        /// Applies the new table name to the configuration.
        /// </summary>
        public void Apply(ITableMappingConfig config)
        {
            config.TableName = _tableNameConverter(config.TableName);
        }
    }
}