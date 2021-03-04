namespace ApigeeSDK.Unit.Tests
{
    public static class StringExtensions
    {
        public static string QuotesToDoubleQuotes(this string text)
        {
            return string.IsNullOrWhiteSpace(text) ? text : text.Replace("'","\"");
        }
    }
}
