using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Unity;
using ApigeeSDK.Services;
using ApigeeSDK.Exceptions;

namespace ApigeeSDK.Unit.Tests
{
    public class HttpServiceAuthenticatedShould
    {
        protected const string Email = "email";
        protected const string Password = "password";
        protected const string OrgName = "organization";
        protected const string EnvName = "test";

        protected Mock<ApigeeClientOptions> _apigeeClientOptionsMock;
        protected Mock<HttpService> _httpServiceMock;
        protected Mock<TokenProvider> _tokenProviderMock;

        public IUnityContainer Container { get; } = new UnityContainer();

        [SetUp]
        public void Setup()
        {
            _apigeeClientOptionsMock = new Mock<ApigeeClientOptions>(Email, Password, OrgName, EnvName);
            Container.RegisterInstance(_apigeeClientOptionsMock.Object);

            _httpServiceMock = new Mock<HttpService>(
                MockBehavior.Strict,
                _apigeeClientOptionsMock.Object);
            Container.RegisterInstance(_httpServiceMock.Object);

            _tokenProviderMock = new Mock<TokenProvider>(
                MockBehavior.Strict,
                _apigeeClientOptionsMock.Object,
                _httpServiceMock.Object);
            Container.RegisterInstance(_tokenProviderMock.Object);

            _tokenProviderMock.Setup(x => x.GetAuthorizationHeader(It.IsAny<bool>())).ReturnsAsync((bool x) =>
            {
                var token = x ? "expiredToken" : "validToken";
                return new KeyValuePair<string, string>("schema", token);
            });

            Container.RegisterSingleton(typeof(HttpServiceAuthenticated), typeof(HttpServiceAuthenticated));
        }

        private HttpServiceAuthenticated Sut
        {
            get
            {
                return Container.Resolve<HttpServiceAuthenticated>();
            }
        }

        [Test]
        public async Task SendAdditionalRequestIfTokenExpired()
        {
            var RESPONSE = "response";

            var numberOfCalls = 0;
            _httpServiceMock
                .Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>()))
                .Returns(() =>
            {
                numberOfCalls++;

                if (numberOfCalls == 1)
                {
                    throw new ApigeeSDKHttpException(HttpStatusCode.Unauthorized, "Unauthorized");
                }

                return Task.FromResult(RESPONSE);
            });

            var response = await Sut.GetAsync(It.IsAny<string>(),
                It.IsAny<IEnumerable<KeyValuePair<string, string>>>());

            Assert.AreEqual(RESPONSE, response);

            _httpServiceMock.Verify(x =>
                x.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<KeyValuePair<string, string>>>()),
                Times.Exactly(2));
            _tokenProviderMock.Verify(x => x.GetAuthorizationHeader(false), Times.Once);
            _tokenProviderMock.Verify(x => x.GetAuthorizationHeader(true), Times.Once);
        }

        [Test]
        public void RethrowWebExceptionIfBadWebResponse()
        {
            _httpServiceMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>()))
                .Callback(() =>
                {
                    throw new ApigeeSDKHttpException(HttpStatusCode.BadRequest, "Bad request");
                });

            Assert.ThrowsAsync<ApigeeSDKHttpException>(async () => await Sut.GetAsync(
                It.IsAny<string>(),
                It.IsAny<IEnumerable<KeyValuePair<string, string>>>())
            );
        }
    }
}
