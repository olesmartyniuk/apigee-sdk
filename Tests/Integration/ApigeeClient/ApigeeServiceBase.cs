using System;
using System.Net;
using RichardSzalay.MockHttp;

namespace ApigeeSDK.Integration.Tests.ApigeeClient
{
    public class ApigeeClientTestsBase
    {
        protected string BaseUrl = "https://api.enterprise.apigee.com";
        protected string AuthUrl = "https://login.apigee.com/oauth/token";
        protected string Email = "email";
        protected string Password = "password";
        protected string OrgName = "organization";
        protected string EnvName = "test";
        protected int RequestTimeOut = 300;
        protected int EntitiesLimit = 1000;

        protected readonly MockHttpMessageHandler MockHttp = new MockHttpMessageHandler();

        public ApigeeClientTestsBase()
        {
            MockHttp
                .When(AuthUrl)
                .Respond(HttpStatusCode.OK, "application/json",
        @"{
                    'access_token': 'access_token',
                    'token_type': 'token_type',
                    'refresh_token': 'refresh_token',
                    'expires_in': 1000,
                    'scope': 'scope',
                    'jti': '00000000-0000-0000-0000-000000000000'
                }");
        }

        protected void RegisterUrl(string url, string json, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            MockHttp
                .When(url)
                .Respond(statusCode, "application/json", json);
        }

        protected ApigeeSDK.ApigeeClient GetApigeeClient()
        {
            var http = MockHttp.ToHttpClient();
            var options = new ApigeeClientOptions(Email, Password, OrgName, EnvName, BaseUrl, AuthUrl,
                TimeSpan.FromSeconds(RequestTimeOut), EntitiesLimit);

            return new ApigeeSDK.ApigeeClient(options, http);
        }
    }
}