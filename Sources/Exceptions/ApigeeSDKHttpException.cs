using System.Net;

namespace ApigeeSDK.Exceptions
{
    public class ApigeeSdkHttpException : ApigeeSdkException
    {
        public HttpStatusCode StatusCode { get; private set; }

        public ApigeeSdkHttpException(HttpStatusCode statusCode, string message = null) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
