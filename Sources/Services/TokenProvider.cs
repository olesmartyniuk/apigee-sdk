using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApigeeSDK.Models;

namespace ApigeeSDK.Services
{
    public class TokenProvider
    {
        private readonly string _authenticationUrl;
        private Token _token;
        private DateTime _requestNewTokenTime;
        private DateTime _refreshTokenTime;
        private bool _originalTokenWasRequested;
        private bool _refreshTokenWasRequested;
        private readonly string _login;
        private readonly string _password;

        private readonly HttpService _httpService;

        public TokenProvider(ApigeeClientOptions options, HttpService httpService)
        {
            _authenticationUrl = options.AuthenticationUrl ?? "https://login.apigee.com/oauth/token";
            _login = options.Email;
            _password = options.Password;
            _httpService = httpService;
        }

        public virtual async Task<KeyValuePair<string, string>> GetAuthorizationHeader(bool isExpiredByFact)
        {
            if (isExpiredByFact || !IsTokenValid)
            {
                await UpdateToken();
            }

            return new KeyValuePair<string, string>("Authorization", $"{_token.TokenType} {_token.AccessToken}");
        }

        private async Task UpdateToken()
        {
            if (TokenCanBeRefreshed)
            {
                await RefreshAccessToken();
            }

            if (!IsTokenValid)
            {
                await RequestNewAccessToken();
            }
        }

        private bool IsTokenValid =>
            (_originalTokenWasRequested && !OriginalTokenIsExpired)
            || (_refreshTokenWasRequested && !RefreshedTokenIsExpired);

        private bool OriginalTokenIsExpired =>
            _originalTokenWasRequested
            && TimeSpan.FromSeconds(_token.ExpiresIn) < (DateTime.UtcNow - _requestNewTokenTime);

        private bool RefreshedTokenIsExpired =>
            _refreshTokenWasRequested
            && TimeSpan.FromSeconds(_token.ExpiresIn) < (DateTime.UtcNow - _refreshTokenTime);

        private bool TokenCanBeRefreshed => _originalTokenWasRequested && !_refreshTokenWasRequested;

        private async Task RequestNewAccessToken()
        {
            var headers = new[]
            {
                new KeyValuePair<string, string>("Content-Type","application/x-www-form-urlencoded"),
                new KeyValuePair<string, string>("Accept","application/json;charset=utf-8"),
                new KeyValuePair<string, string>("Authorization","Basic ZWRnZWNsaTplZGdlY2xpc2VjcmV0")
            };

            var formContent = new[]
            {
                new KeyValuePair<string, string>("username", _login),
                new KeyValuePair<string, string>("password", _password),
                new KeyValuePair<string, string>("grant_type", "password")
            };

            var content = await _httpService.PostAsync(_authenticationUrl, headers, formContent);

            _token = Newtonsoft.Json.JsonConvert.DeserializeObject<Token>(content);
            _refreshTokenWasRequested = false;
            _requestNewTokenTime = DateTime.UtcNow;
            _originalTokenWasRequested = true;
        }

        private async Task RefreshAccessToken()
        {
            var headers = new[]
            {
                new KeyValuePair<string, string>("Content-Type","application/x-www-form-urlencoded"),
                new KeyValuePair<string, string>("Accept","application/json;charset=utf-8"),
                new KeyValuePair<string, string>("Authorization","Basic ZWRnZWNsaTplZGdlY2xpc2VjcmV0")
            };

            var formContent = new[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", _token.RefreshToken),
            };

            var content = await _httpService.PostAsync(_authenticationUrl, headers, formContent);
            
            _token = Newtonsoft.Json.JsonConvert.DeserializeObject<Token>(content);
            _refreshTokenWasRequested = true;
            _refreshTokenTime = DateTime.UtcNow;            
        }
    }
}
