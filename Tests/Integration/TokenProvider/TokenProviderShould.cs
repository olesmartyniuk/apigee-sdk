using System;
using System.Net;
using System.Threading.Tasks;
using ApigeeSDK.Integration.Tests.ApigeeClient;
using ApigeeSDK.Services;
using RichardSzalay.MockHttp;
using Xunit;

namespace ApigeeSDK.Integration.Tests.TokenProvider
{
    public class TokenProviderShould : ApigeeClientTestsBase
    {
        private Services.TokenProvider CreateTokenProvider()
        {
            var http = MockHttp.ToHttpClient();
            var options = new ApigeeClientOptions(Email, Password, OrgName, EnvName, BaseUrl, AuthUrl,
                TimeSpan.FromSeconds(RequestTimeOut), EntitiesLimit);
            var httpService = new HttpService(http);

            return new Services.TokenProvider(options, httpService);
        }

        [Fact]
        public async Task ProvideAccessToken()
        {
            MockHttp.Clear();

            MockHttp
                .When(AuthUrl)
                .Respond(HttpStatusCode.OK, "application/json",
                    @"{
                    'access_token': 'access_token_value',
                    'token_type': 'token_type_value',
                    'refresh_token': 'refresh_token_value',
                    'expires_in': 1000,
                    'scope': 'scim.me openid password.write approvals.me oauth.approvals',
                    'jti': '11111111-1111-1111-1111-111111111111'
                }");

            var header = await CreateTokenProvider().GetAuthorizationHeader(false);

            Assert.Equal("Authorization", header.Key);
            Assert.Equal("token_type_value access_token_value", header.Value);
        }

        [Fact]
        public async Task DoNotRefreshTokenIfNotExpired()
        {
            MockHttp.Clear();

            var getToken = MockHttp.When(AuthUrl)
                .WithFormData("grant_type", "password")
                .Respond(HttpStatusCode.OK, "application/json",
                    @"{
                        'access_token': 'old_token',
                        'token_type': 'token_type',
                        'refresh_token': 'refresh_token',
                        'expires_in': 1000,
                        'scope': 'scope',
                        'jti': '00000000-0000-0000-0000-000000000000'
                    }");

            var refreshToken = MockHttp.When(AuthUrl)
                .WithFormData("grant_type", "refresh_token")
                .Respond(HttpStatusCode.OK, "application/json",
        @"{
                        'access_token': 'new_token',
                        'token_type': 'token_type',
                        'refresh_token': 'refresh_token',
                        'expires_in': 1000,
                        'scope': 'scope',
                        'jti': '00000000-0000-0000-0000-000000000000'
                    }");

            var provider = CreateTokenProvider();

            await provider.GetAuthorizationHeader(false);

            var header = await provider.GetAuthorizationHeader(false);

            Assert.Equal("Authorization", header.Key);
            Assert.Equal("token_type old_token", header.Value);

            Assert.Equal(1, MockHttp.GetMatchCount(getToken));
            Assert.Equal(0, MockHttp.GetMatchCount(refreshToken));
        }

        [Fact]
        public async Task RefreshTokenIfExpired()
        {
            MockHttp.Clear();

            var getToken = MockHttp.When(AuthUrl)
                .WithFormData("grant_type", "password")
                .Respond(HttpStatusCode.OK, "application/json",
                    @"{
                        'access_token': 'old_token',
                        'token_type': 'token_type',
                        'refresh_token': 'refresh_token',
                        'expires_in': 0,
                        'scope': 'scope',
                        'jti': '00000000-0000-0000-0000-000000000000'
                    }");

            var refreshToken = MockHttp.When(AuthUrl)
                .WithFormData("grant_type", "refresh_token")
                .Respond(HttpStatusCode.OK, "application/json",
                    @"{
                        'access_token': 'new_token',
                        'token_type': 'token_type',
                        'refresh_token': 'refresh_token',
                        'expires_in': 1000,
                        'scope': 'scope',
                        'jti': '00000000-0000-0000-0000-000000000000'
                    }");

            var provider = CreateTokenProvider();

            await provider.GetAuthorizationHeader(false);

            var header = await provider.GetAuthorizationHeader(false);

            Assert.Equal("Authorization", header.Key);
            Assert.Equal("token_type new_token", header.Value);

            Assert.Equal(1, MockHttp.GetMatchCount(getToken));
            Assert.Equal(1, MockHttp.GetMatchCount(refreshToken));
        }

        [Fact]
        public async Task RefreshTokenIfNotExpiredButForcedByUser()
        {
            MockHttp.Clear();

            var getToken = MockHttp.When(AuthUrl)
                .WithFormData("grant_type", "password")
                .Respond(HttpStatusCode.OK, "application/json",
                    @"{
                        'access_token': 'old_token',
                        'token_type': 'token_type',
                        'refresh_token': 'refresh_token',
                        'expires_in': 1000,
                        'scope': 'scope',
                        'jti': '00000000-0000-0000-0000-000000000000'
                    }");

            var refreshToken = MockHttp.When(AuthUrl)
                .WithFormData("grant_type", "refresh_token")
                .Respond(HttpStatusCode.OK, "application/json",
                    @"{
                        'access_token': 'new_token',
                        'token_type': 'token_type',
                        'refresh_token': 'refresh_token',
                        'expires_in': 1000,
                        'scope': 'scope',
                        'jti': '00000000-0000-0000-0000-000000000000'
                    }");

            var provider = CreateTokenProvider();

            await provider.GetAuthorizationHeader(false);

            var header = await provider.GetAuthorizationHeader(true);

            Assert.Equal("Authorization", header.Key);
            Assert.Equal("token_type new_token", header.Value);

            Assert.Equal(1, MockHttp.GetMatchCount(getToken));
            Assert.Equal(1, MockHttp.GetMatchCount(refreshToken));
        }

        [Fact]
        public async Task RequestNewTokenWhenRefreshedTokenExpired()
        {
            MockHttp.Clear();

            var getToken = MockHttp.When(AuthUrl)
                .WithFormData("grant_type", "password")
                .Respond(HttpStatusCode.OK, "application/json",
                    @"{
                        'access_token': 'old_token',
                        'token_type': 'token_type',
                        'refresh_token': 'refresh_token',
                        'expires_in': 0,
                        'scope': 'scope',
                        'jti': '00000000-0000-0000-0000-000000000000'
                    }");

            var refreshToken = MockHttp.When(AuthUrl)
                .WithFormData("grant_type", "refresh_token")
                .Respond(HttpStatusCode.OK, "application/json",
                    @"{
                        'access_token': 'new_token',
                        'token_type': 'token_type',
                        'refresh_token': 'refresh_token',
                        'expires_in': 0,
                        'scope': 'scope',
                        'jti': '00000000-0000-0000-0000-000000000000'
                    }");

            var provider = CreateTokenProvider();

            await provider.GetAuthorizationHeader(false);

            var header = await provider.GetAuthorizationHeader(false);

            Assert.Equal("Authorization", header.Key);
            Assert.Equal("token_type old_token", header.Value);

            Assert.Equal(2, MockHttp.GetMatchCount(getToken));
            Assert.Equal(1, MockHttp.GetMatchCount(refreshToken));
        }

        [Fact]
        public async Task DoNotRequestNewTokenWhenRefreshedTokenIsNotExpired()
        {
            MockHttp.Clear();

            var getToken = MockHttp.When(AuthUrl)
                .WithFormData("grant_type", "password")
                .Respond(HttpStatusCode.OK, "application/json",
                    @"{
                        'access_token': 'old_token',
                        'token_type': 'token_type',
                        'refresh_token': 'refresh_token',
                        'expires_in': 0,
                        'scope': 'scope',
                        'jti': '00000000-0000-0000-0000-000000000000'
                    }");

            var refreshToken = MockHttp.When(AuthUrl)
                .WithFormData("grant_type", "refresh_token")
                .Respond(HttpStatusCode.OK, "application/json",
                    @"{
                        'access_token': 'new_token',
                        'token_type': 'token_type',
                        'refresh_token': 'refresh_token',
                        'expires_in': 1000,
                        'scope': 'scope',
                        'jti': '00000000-0000-0000-0000-000000000000'
                    }");

            var provider = CreateTokenProvider();

            await provider.GetAuthorizationHeader(false);
            await provider.GetAuthorizationHeader(false);

            var header = await provider.GetAuthorizationHeader(false);

            Assert.Equal("Authorization", header.Key);
            Assert.Equal("token_type new_token", header.Value);

            Assert.Equal(1, MockHttp.GetMatchCount(getToken));
            Assert.Equal(1, MockHttp.GetMatchCount(refreshToken));
        }
    }
}
