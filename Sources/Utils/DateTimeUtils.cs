using System;

namespace ApigeeSDK.Utils
{
    public static class DateTimeUtils
    {
        public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return epoch.AddMilliseconds(unixTimeStamp);
        }
    }
}