using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity;
using ApigeeSDK.Services;
using Xunit;

namespace ApigeeSDK.Unit.Tests
{
    public class TokenProviderShould
    {
        protected const string Email = "email";
        protected const string Password = "password";
        protected const string OrgName = "organization";
        protected const string EnvName = "test";
        protected const int RequestTimeOut = 300;

        protected Mock<ApigeeClientOptions> _apigeeClientOptionsMock;
        protected Mock<HttpService> _httpServiceMock;

        public IUnityContainer Container { get; } = new UnityContainer();

        public TokenProviderShould()
        {
            _apigeeClientOptionsMock = new Mock<ApigeeClientOptions>(Email, Password, OrgName, EnvName);
            Container.RegisterInstance(_apigeeClientOptionsMock.Object);

            _httpServiceMock = new Mock<HttpService>(
                MockBehavior.Strict,
                _apigeeClientOptionsMock.Object);
            Container.RegisterInstance(_httpServiceMock.Object);

            Container.RegisterSingleton<TokenProvider>();
        }
        
        private TokenProvider Sut
        {
            get
            {
                return Container.Resolve<TokenProvider>();
            }
        }

        [Fact]
        public async Task ProvideAccessToken()
        {
            SetupPostAsync("access_token_value", 1799);

            var header = await Sut.GetAuthorizationHeader(false);

            Assert.Equal("Authorization", header.Key);
            Assert.Equal("token_type_value access_token_value", header.Value);
        }

        [Fact]
        public async Task DoNotRefreshTokenIfNotExpired()
        {
            SetupPostAsync("access_token_value", 1799);

            var header = await Sut.GetAuthorizationHeader(false);

            SetupPostAsync("access_token_value2", 1799);

            header = await Sut.GetAuthorizationHeader(false);

            Assert.Equal("Authorization", header.Key);
            Assert.Equal("token_type_value access_token_value", header.Value);
        }

        [Fact]
        public async Task RefreshTokenIfExpired()
        {
            SetupPostAsync("access_token_value", 0);

            var header = await Sut.GetAuthorizationHeader(false);

            SetupPostAsync("access_token_value2", 1799);

            header = await Sut.GetAuthorizationHeader(false);

            Assert.Equal("Authorization", header.Key);
            Assert.Equal("token_type_value access_token_value2", header.Value);
        }

        [Fact]
        public async Task RefreshTokenIfNotExpiredButForcedByUser()
        {
            SetupPostAsync("access_token_value", 1799);

            var header = await Sut.GetAuthorizationHeader(false);

            SetupPostAsync("access_token_value2", 1799);

            header = await Sut.GetAuthorizationHeader(true);

            Assert.Equal("Authorization", header.Key);
            Assert.Equal("token_type_value access_token_value2", header.Value);
        }

        [Fact]
        public async Task RequestNewTokenWhenRefreshedTokenExpired()
        {
            SetupPostAsync("access_token_value", 0);

            var header = await Sut.GetAuthorizationHeader(false);

            SetupPostAsync("access_token_value2", 0);

            header = await Sut.GetAuthorizationHeader(false);

            SetupPostAsync("access_token_value3", 1799);

            header = await Sut.GetAuthorizationHeader(false);

            Assert.Equal("Authorization", header.Key);
            Assert.Equal("token_type_value access_token_value3", header.Value);
        }

        [Fact]
        public async Task DoNotRequestNewTokenWhenRefreshedTokenIsNotExpired()
        {
            SetupPostAsync("access_token_value", 0);

            var header = await Sut.GetAuthorizationHeader(false);

            SetupPostAsync("access_token_value2", 1799);

            header = await Sut.GetAuthorizationHeader(false);

            SetupPostAsync("access_token_value3", 1799);

            header = await Sut.GetAuthorizationHeader(false);

            Assert.Equal("Authorization", header.Key);
            Assert.Equal("token_type_value access_token_value2", header.Value);
        }

        private void SetupPostAsync(string tokenValue, int tokenExpiresSeconds)
        {
            _httpServiceMock.Setup(x => x.PostAsync("https://login.apigee.com/oauth/token",
                    It.IsAny<KeyValuePair<string, string>[]>(),
                    It.IsAny<KeyValuePair<string, string>[]>()))
                .Returns(Task.FromResult($@"{{
                        ""access_token"": ""{tokenValue}"",
                        ""token_type"": ""token_type_value"",
                        ""refresh_token"": ""refresh_token_value"",
                        ""expires_in"": {tokenExpiresSeconds},
                        ""scope"": ""scim.me openid password.write approvals.me oauth.approvals"",
                        ""jti"": ""11111111-1111-1111-1111-111111111111""
                    }}"));
        }
    }
}
