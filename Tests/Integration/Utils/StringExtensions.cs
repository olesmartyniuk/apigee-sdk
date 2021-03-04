namespace ApigeeSDK.Integration.Tests.Utils
{
    public static class StringExtensions
    {
        public static string QuotesToDoubleQuotes(this string text)
        {
            return string.IsNullOrWhiteSpace(text) ? text : text.Replace("'","\"");
        }
    }
}
