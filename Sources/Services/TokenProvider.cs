using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApigeeSDK.Models;

namespace ApigeeSDK.Services
{
    public class TokenProvider
    {
        private readonly string _authenticationUrl = "https://login.apigee.com/oauth/token";
        private Token token;
        private DateTime requestNewTokenTime;
        private DateTime refreshTokenTime;
        private bool originalTokenWasRequested;
        private bool refreshTokenWasRequested;
        private readonly string _login;
        private readonly string _password;

        private readonly HttpService _httpService;

        public TokenProvider(ApigeeClientOptions options, HttpService httpService)
        {
            _authenticationUrl = options.AuthenticationUrl;
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

            return new KeyValuePair<string, string>("Authorization", $"{token.TokenType} {token.AccessToken}");
        }

        private async Task UpdateToken()
        {
            var updateTokenSucceed = false;
            if (TokenCanBeRefreshed)
            {
                await RefreshAccessToken();
            }

            if (!updateTokenSucceed)
            {
                await RequestNewAccessToken();
            }
        }

        private bool IsTokenValid
        {
            get
            {
                return (originalTokenWasRequested && !OriginalTokenIsExpired)
                       || (refreshTokenWasRequested && !RefreshedTokenIsExpired);
            }
        }
        private bool OriginalTokenIsExpired
        {
            get
            {
                return originalTokenWasRequested
                       && TimeSpan.FromSeconds(token.ExpiresIn) < (DateTime.UtcNow - requestNewTokenTime);
            }
        }

        private bool RefreshedTokenIsExpired
        {
            get
            {
                return refreshTokenWasRequested
                       && TimeSpan.FromSeconds(token.ExpiresIn) < (DateTime.UtcNow - refreshTokenTime);
            }
        }

        private bool TokenCanBeRefreshed
        {
            get { return originalTokenWasRequested && !refreshTokenWasRequested; }
        }

        private async Task RequestNewAccessToken()
        {
            var headers = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("Content-Type","application/x-www-form-urlencoded"),
                new KeyValuePair<string, string>("Accept","application/json;charset=utf-8"),
                new KeyValuePair<string, string>("Authorization","Basic ZWRnZWNsaTplZGdlY2xpc2VjcmV0")
            };

            var formContent = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("username", _login),
                new KeyValuePair<string, string>("password", _password),
                new KeyValuePair<string, string>("grant_type", "password")
            };

            var content = await _httpService.PostAsync(_authenticationUrl, headers, formContent);

            token = Newtonsoft.Json.JsonConvert.DeserializeObject<Token>(content);
            refreshTokenWasRequested = false;
            requestNewTokenTime = DateTime.UtcNow;
            originalTokenWasRequested = true;
        }

        private async Task RefreshAccessToken()
        {
            var headers = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("Content-Type","application/x-www-form-urlencoded"),
                new KeyValuePair<string, string>("Accept","application/json;charset=utf-8"),
                new KeyValuePair<string, string>("Authorization","Basic ZWRnZWNsaTplZGdlY2xpc2VjcmV0")
            };

            var formContent = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", token.RefreshToken),
            };

            var content = await _httpService.PostAsync(_authenticationUrl, headers, formContent);
            
            token = Newtonsoft.Json.JsonConvert.DeserializeObject<Token>(content);
            refreshTokenWasRequested = true;
            refreshTokenTime = DateTime.UtcNow;            
        }
    }
}
