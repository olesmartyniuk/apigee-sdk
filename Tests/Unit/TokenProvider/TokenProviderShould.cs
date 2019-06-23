using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Unity;
using Unity.Injection;
using ApigeeSDK.Services;

namespace ApigeeSDK.Unit.Tests
{
    public class TokenProviderShould
    {
        public IUnityContainer Container { get; } = new UnityContainer();

        private Mock<HttpService> httpServiceMoq;

        [SetUp]
        public void Init()
        {
            this.httpServiceMoq = new Mock<HttpService>(Timeout.InfiniteTimeSpan);
            this.Container.RegisterInstance(typeof(HttpService), httpServiceMoq.Object);
            this.Container.RegisterType(
                typeof(TokenProvider),
                typeof(TokenProvider),
                new InjectionConstructor(typeof(HttpService), "userId", "userPassword"));
        }

        [Test]
        public async Task ProvideAccessToken()
        {
            this.SetupPostAsync("access_token_value", 1799);

            var tp = Container.Resolve<TokenProvider>();

            var header = await tp.GetAuthorizationHeader(false);

            Assert.AreEqual("Authorization", header.Key);
            Assert.AreEqual("token_type_value access_token_value", header.Value);
        }

        [Test]
        public async Task DoNotRefreshTokenIfNotExpired()
        {
            this.SetupPostAsync("access_token_value", 1799);

            var tp = Container.Resolve<TokenProvider>();

            var header = await tp.GetAuthorizationHeader(false);

            this.SetupPostAsync("access_token_value2", 1799);

            header = await tp.GetAuthorizationHeader(false);

            Assert.AreEqual("Authorization", header.Key);
            Assert.AreEqual("token_type_value access_token_value", header.Value);
        }

        [Test]
        public async Task RefreshTokenIfExpired()
        {
            this.SetupPostAsync("access_token_value", 0);

            var tp = Container.Resolve<TokenProvider>();

            var header = await tp.GetAuthorizationHeader(false);

            this.SetupPostAsync("access_token_value2", 1799);

            header = await tp.GetAuthorizationHeader(false);

            Assert.AreEqual("Authorization", header.Key);
            Assert.AreEqual("token_type_value access_token_value2", header.Value);
        }

        [Test]
        public async Task RefreshTokenIfNotExpiredButForcedByUser()
        {
            this.SetupPostAsync("access_token_value", 1799);

            var tp = Container.Resolve<TokenProvider>();

            var header = await tp.GetAuthorizationHeader(false);

            this.SetupPostAsync("access_token_value2", 1799);

            header = await tp.GetAuthorizationHeader(true);

            Assert.AreEqual("Authorization", header.Key);
            Assert.AreEqual("token_type_value access_token_value2", header.Value);
        }

        [Test]
        public async Task RequestNewTokenWhenRefreshedTokenExpired()
        {
            this.SetupPostAsync("access_token_value", 0);

            var tp = Container.Resolve<TokenProvider>();

            var header = await tp.GetAuthorizationHeader(false);

            this.SetupPostAsync("access_token_value2", 0);

            header = await tp.GetAuthorizationHeader(false);

            this.SetupPostAsync("access_token_value3", 1799);

            header = await tp.GetAuthorizationHeader(false);

            Assert.AreEqual("Authorization", header.Key);
            Assert.AreEqual("token_type_value access_token_value3", header.Value);
        }

        [Test]
        public async Task DoNotRequestNewTokenWhenRefreshedTokenIsNotExpired()
        {
            this.SetupPostAsync("access_token_value", 0);

            var tp = Container.Resolve<TokenProvider>();

            var header = await tp.GetAuthorizationHeader(false);

            this.SetupPostAsync("access_token_value2", 1799);

            header = await tp.GetAuthorizationHeader(false);

            this.SetupPostAsync("access_token_value3", 1799);

            header = await tp.GetAuthorizationHeader(false);

            Assert.AreEqual("Authorization", header.Key);
            Assert.AreEqual("token_type_value access_token_value2", header.Value);
        }

        private void SetupPostAsync(string tokenValue, int tokenExpiresSeconds)
        {
            httpServiceMoq.Setup(x => x.PostAsync("https://login.apigee.com/oauth/token",
                    It.IsAny<KeyValuePair<string, string>[]>(),
                    It.IsAny<KeyValuePair<string, string>[]>()))
                .Returns(Task.FromResult((true, HttpStatusCode.Accepted, $@"{{
                        ""access_token"": ""{tokenValue}"",
                        ""token_type"": ""token_type_value"",
                        ""refresh_token"": ""refresh_token_value"",
                        ""expires_in"": {tokenExpiresSeconds},
                        ""scope"": ""scim.me openid password.write approvals.me oauth.approvals"",
                        ""jti"": ""11111111-1111-1111-1111-111111111111""
                    }}")));
        }
    }
}
