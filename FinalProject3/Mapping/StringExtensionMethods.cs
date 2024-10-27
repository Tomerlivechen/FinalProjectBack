namespace FinalProject3.Mapping
{
    public static class StringExtensionMethods
    {
        public static string Capitelize(this string value)
        {
            string str = char.ToUpper(value[0]) + value.Substring(1);
            return str;
        }
    }
}
