using System.Text.RegularExpressions;

namespace Cassandra.Mapping.Conventions
{
    /// <summary>
    /// Some utility functions for converting names.
    /// </summary>
    public static class NameConverters
    {
        private static readonly Regex CamelCaseRegex = new Regex(@"((?<=.)[A-Z][a-z]*)|((?<=[a-zA-Z])\d+)", RegexOptions.Compiled);

        /// <summary>
        /// Converts the provided name to all upper case.  If currentName is null, returns null.
        /// </summary>
        public static string ToUpper(string currentName)
        {
            if (currentName == null) return null;
            return currentName.ToUpper();
        }

        /// <summary>
        /// Converts the provided name to all lower case.  If currentName is null, returns null.
        /// </summary>
        public static string ToLower(string currentName)
        {
            if (currentName == null) return null;
            return currentName.ToLower();
        }

        /// <summary>
        /// Inserts underscores into the camel case name specified anywhere there is a word break.  For example: "IsStatic" -> "Is_Static", and
        /// "wasJobFinished" -> "was_Job_Finished".  Also considers digits (0-9) as a word break, so for example: "OverBy400" -> "Over_By_400",
        /// "is700Above" -> "is_700_Above", and "is700above" -> "is_700above".  If currentName is null, returns null.
        /// </summary>
        public static string CamelCaseToUnderscore(string currentName)
        {
            if (currentName == null) return null;
            return CamelCaseRegex.Replace(currentName, "_$1$2");
        }
    }
}