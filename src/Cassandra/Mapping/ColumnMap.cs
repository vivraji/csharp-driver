using System;

namespace Cassandra.Mapping
{
    /// <summary>
    /// A class for defining how a property or field on a POCO is mapped to a column via a fluent-style interface.
    /// </summary>
    public class ColumnMap
    {
        private string _columnName;
        private Type _columnType;
        private bool _ignore;
        private readonly bool _isExplicitlyDefined;
        private bool _secondaryIndex;
        private bool _isCounter;
        private bool _isStatic;
        
        /// <summary>
        /// Creates a new ColumnMap for the property/field specified by the MemberInfo.
        /// </summary>
        public ColumnMap(bool isExplicitlyDefined)
        {
            _isExplicitlyDefined = isExplicitlyDefined;
        }

        internal void ApplyTo(IColumnMappingConfig columnConfig)
        {
            // Override values on the column's config with our values
            if (_columnName != null)
                columnConfig.ColumnName = _columnName;

            if (_columnType != null)
                columnConfig.ColumnType = _columnType;

            columnConfig.Ignore = _ignore;
            columnConfig.IsCounter = _isCounter;
            columnConfig.IsExplicitlyDefined = _isExplicitlyDefined;
            columnConfig.IsStatic = _isStatic;
            columnConfig.SecondaryIndex = _secondaryIndex;
        }

        /// <summary>
        /// Tells the mapper to ignore this property/field when mapping.
        /// </summary>
        public ColumnMap Ignore()
        {
            _ignore = true;
            return this;
        }

        /// <summary>
        /// Tells the mapper to use the column name specified when mapping the property/field.
        /// </summary>
        public ColumnMap WithName(string columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName)) throw new ArgumentNullException("columnName");

            _columnName = columnName;
            return this;
        }

        /// <summary>
        /// Tells the mapper to convert the data in the property or field to the Type specified when doing an INSERT or UPDATE (i.e. the
        /// column type in Cassandra).  (NOTE: This does NOT affect the Type when fetching/SELECTing data from the database.)
        /// </summary>
        public ColumnMap WithDbType(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            _columnType = type;
            return this;
        }

        /// <summary>
        /// Tells the mapper to convert the data in the property or field to Type T when doing an INSERT or UPDATE (i.e. the
        /// column type in Cassandra).  (NOTE: This does NOT affect the Type when fetching/SELECTing data from the database.)
        /// </summary>
        public ColumnMap WithDbType<T>()
        {
            _columnType = typeof (T);
            return this;
        }

        /// <summary>
        /// Tells the mapper that this column is defined also as a secondary index
        /// </summary>
        /// <returns></returns>
        public ColumnMap WithSecondaryIndex()
        {
            _secondaryIndex = true;
            return this;
        }

        /// <summary>
        /// Tells the mapper that this is a counter column
        /// </summary>
        public ColumnMap AsCounter()
        {
            _isCounter = true;
            return this;
        }

        /// <summary>
        /// Tells the mapper that this is a static column
        /// </summary>
        public ColumnMap AsStatic()
        {
            _isStatic = true;
            return this;
        }
    }
}