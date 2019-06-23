using System.Net;

namespace ApigeeSDK.Exceptions
{
    public class ApigeeSDKHttpException : ApigeeSDKException
    {
        public HttpStatusCode StatusCode { get; private set; }

        public ApigeeSDKHttpException(HttpStatusCode statusCode, string message = null) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
