using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ApigeeSDK.Exceptions;

namespace ApigeeSDK.Services
{
    public class HttpServiceAuthenticated
    {
        private readonly HttpService _httpService;
        private readonly TokenProvider _tokenProvider;

        public HttpServiceAuthenticated(ApigeeClientOptions options, HttpClient http)
        {
            _httpService = new HttpService(http);
            _tokenProvider = new TokenProvider(options, _httpService);
        }

        public virtual async Task<string> GetAsync(string url,
            IEnumerable<KeyValuePair<string, string>> rawHeaders = null)
        {
            Task<string> Action(IEnumerable<KeyValuePair<string, string>> headers)
            {
                return _httpService.GetAsync(url, headers);
            }

            return await CallWithAuthorizationHeader(rawHeaders, Action);
        }

        public virtual async Task<string> PostJsonAsync(string url,
            IEnumerable<KeyValuePair<string, string>> rawHeaders,
            string json)
        {
            Task<string> Action(IEnumerable<KeyValuePair<string, string>> headers)
            {
                return _httpService.PostJsonAsync(url, headers, json);
            }

            return await CallWithAuthorizationHeader(rawHeaders, Action);
        }

        public virtual async Task<string> PostFileAsync(string url,
            IEnumerable<KeyValuePair<string, string>> rawHeaders,
            string filePath)
        {
            Task<string> Action(IEnumerable<KeyValuePair<string, string>> headers)
            {
                return _httpService.PostFileAsync(url, headers, filePath);
            }

            return await CallWithAuthorizationHeader(rawHeaders, Action);
        }

        public virtual async Task<string> PostAsync(string url,
            IEnumerable<KeyValuePair<string, string>> rawHeaders)
        {
            Task<string> Action(IEnumerable<KeyValuePair<string, string>> headers)
            {
                return _httpService.PostAsync(url, headers, null);
            }

            return await CallWithAuthorizationHeader(rawHeaders, Action);
        }

        private delegate Task<string> HttpActionDelegate(IEnumerable<KeyValuePair<string, string>> headers); 

        public virtual async Task<string> DeleteAsync(string url,
            IEnumerable<KeyValuePair<string, string>> rawHeaders)
        {
            Task<string> Action(IEnumerable<KeyValuePair<string, string>> headers)
            {
                return _httpService.DeleteAsync(url, headers, null);
            }

            return await CallWithAuthorizationHeader(rawHeaders, Action);
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
            catch (ApigeeSdkHttpException e)
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