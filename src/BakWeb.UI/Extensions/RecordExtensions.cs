using Umbraco.Forms.Core.Interfaces;

namespace BakWeb.Extensions
{
    public static class RecordExtensions
    {
        public static string GetFieldValueAsString(this IRecord record, string fieldName)
        {
            return record.RecordFields.FirstOrDefault(x =>
                string.Equals(x.Value.Alias, fieldName, StringComparison.InvariantCultureIgnoreCase)).Value.ValuesAsString();
        }
    }
}
