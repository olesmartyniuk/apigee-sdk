using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ApigeeSDK.Exceptions;

namespace ApigeeSDK.Services
{
    public class HttpServiceAuthenticated
    {
        private HttpService _httpService;
        private TokenProvider _tokenProvider;

        public HttpServiceAuthenticated(HttpService httpService, TokenProvider tokenProvider)
        {
            _httpService = httpService;
            _tokenProvider = tokenProvider;
        }

        public virtual async Task<string> GetAsync(string url,
            IEnumerable<KeyValuePair<string, string>> rawHeaders = null)
        {
            Task<string> action(IEnumerable<KeyValuePair<string, string>> headers)
            {
                return _httpService.GetAsync(url, headers);
            }

            return await CallWithAuthorizationHeader(rawHeaders, action);
        }

        public virtual async Task<string> PostJsonAsync(string url,
            IEnumerable<KeyValuePair<string, string>> rawHeaders,
            string json)
        {
            Task<string> action(IEnumerable<KeyValuePair<string, string>> headers)
            {
                return _httpService.PostJsonAsync(url, headers, json);
            }

            return await CallWithAuthorizationHeader(rawHeaders, action);
        }

        public virtual async Task<string> PostFileAsync(string url,
            IEnumerable<KeyValuePair<string, string>> rawHeaders,
            string filePath)
        {
            Task<string> action(IEnumerable<KeyValuePair<string, string>> headers)
            {
                return _httpService.PostFileAsync(url, headers, filePath);
            }

            return await CallWithAuthorizationHeader(rawHeaders, action);
        }

        public virtual async Task<string> PostAsync(string url,
            IEnumerable<KeyValuePair<string, string>> rawHeaders)
        {
            Task<string> action(IEnumerable<KeyValuePair<string, string>> headers)
            {
                return _httpService.PostAsync(url, headers, null);
            }

            return await CallWithAuthorizationHeader(rawHeaders, action);
        }

        private delegate Task<string> HttpActionDelegate(IEnumerable<KeyValuePair<string, string>> headers); 

        public virtual async Task<string> DeleteAsync(string url,
            IEnumerable<KeyValuePair<string, string>> rawHeaders)
        {
            Task<string> action(IEnumerable<KeyValuePair<string, string>> headers)
            {
                return _httpService.DeleteAsync(url, headers, null);
            }

            return await CallWithAuthorizationHeader(rawHeaders, action);
        }

        private async Task<string> CallWithAuthorizationHeader
            (IEnumerable<KeyValuePair<string, string>> rawHeaders, 
            Func<IEnumerable<KeyValuePair<string, string>>, Task<string>> func)
        {
            var headers = PrepareHeaders(rawHeaders);
            var authHeader = await _tokenProvider.GetAuthorizationHeader(false);
            headers.Add(authHeader);

            try
            {
                return await func(headers);
            }
            catch (ApigeeSDKHttpException e)
                when (e.StatusCode == HttpStatusCode.Unauthorized)
            {
                headers = PrepareHeaders(rawHeaders);
                authHeader = await _tokenProvider.GetAuthorizationHeader(true);
                headers.Add(authHeader);
                return await func(headers);
            }
        }

        private List<KeyValuePair<string, string>> PrepareHeaders(IEnumerable<KeyValuePair<string, string>> rawHeaders)
        {
            return rawHeaders == null ? 
                new List<KeyValuePair<string, string>>() :
                rawHeaders.ToList();
        }
    }
}