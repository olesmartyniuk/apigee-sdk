using System;

namespace ApigeeSDK.Exceptions
{
    public class ApigeeSDKException : Exception
    {
        public ApigeeSDKException(string message) : base(message)
        {
        }
    }
}
