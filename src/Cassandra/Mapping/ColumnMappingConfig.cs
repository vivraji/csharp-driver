using System;
using System.Reflection;

namespace Cassandra.Mapping
{
    /// <summary>
    /// Represents mutable configuration about how to map a property/field to a column in C*.
    /// </summary>
    internal class ColumnMappingConfig : IColumnMappingConfig
    {
        private readonly Type _pocoType;
        private readonly MemberInfo _memberInfo;
        private readonly Type _memberInfoType;

        private string _columnName;
        private Type _columnType;

        public Type PocoType
        {
            get { return _pocoType; }
        }

        public MemberInfo MemberInfo
        {
            get { return _memberInfo; }
        }

        public Type MemberInfoType
        {
            get { return _memberInfoType; }
        }

        public string ColumnName
        {
            get { return _columnName; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentOutOfRangeException("value", "ColumnName cannot be set to null or whitespace.");
                _columnName = value;
            }
        }

        public Type ColumnType
        {
            get { return _columnType; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value", "ColumnType cannot be set to null.");
                _columnType = value;
            }
        }

        public bool Ignore { get; set; }

        public bool IsExplicitlyDefined { get; set; }

        public bool SecondaryIndex { get; set; }

        public bool IsCounter { get; set; }

        public bool IsStatic { get; set; }
        
        public ColumnMappingConfig(Type pocoType, FieldInfo fieldInfo)
            : this(pocoType, fieldInfo, fieldInfo.FieldType)
        {
        }

        public ColumnMappingConfig(Type pocoType, PropertyInfo propertyInfo)
            : this(pocoType, propertyInfo, propertyInfo.PropertyType)
        {
        }

        private ColumnMappingConfig(Type pocoType, MemberInfo memberInfo, Type memberInfoType)
        {
            if (pocoType == null) throw new ArgumentNullException("pocoType");
            if (memberInfo == null) throw new ArgumentNullException("memberInfo");
            if (memberInfoType == null) throw new ArgumentNullException("memberInfoType");

            _pocoType = pocoType;
            _memberInfo = memberInfo;
            _memberInfoType = memberInfoType;

            // Default assumptions
            _columnName = memberInfo.Name;
            _columnType = memberInfoType;
            Ignore = false;
            IsExplicitlyDefined = false;
            SecondaryIndex = false;
            IsCounter = false;
            IsStatic = false;
        }
    }
}
