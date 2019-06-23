namespace ApigeeSDK.Unit.Tests
{
    public static class StringExtensions
    {
        public static bool IsNullOrWhiteSpace(this string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }

        public static bool IsNullOrEmpty(this string text)
        {
            return string.IsNullOrEmpty(text);
        }

        public static string IfNullOrWhiteSpace(this string text, string replaceValue)
        {
            return string.IsNullOrWhiteSpace(text) ? replaceValue : text;
        }

        public static string QuotesToDoubleQuotes(this string text)
        {
            return string.IsNullOrWhiteSpace(text) ? text : text.Replace("'","\"");
        }
    }
}
