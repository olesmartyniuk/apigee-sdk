using System;

namespace ApigeeSDK.Exceptions
{
    public class ApigeeSdkException : Exception
    {
        public ApigeeSdkException(string message) : base(message)
        {
        }
    }
}
