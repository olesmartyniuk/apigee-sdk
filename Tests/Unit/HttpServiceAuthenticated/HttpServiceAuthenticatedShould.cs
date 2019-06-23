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
        private Mock<TokenProvider> _tokenProviderMock;
        private Mock<HttpService> _httpServiceMock;
        public IUnityContainer Container { get; } = new UnityContainer();

        [SetUp]
        public void Setup()
        {
            _tokenProviderMock = new Mock<TokenProvider>();
            _tokenProviderMock.Setup(x => x.GetAuthorizationHeader(It.IsAny<bool>())).ReturnsAsync((bool x) =>
            {
                string token = x ? "expiredToken" : "validToken";
                return new KeyValuePair<string, string>("schema", token);
            });

            _httpServiceMock = new Mock<HttpService>(Timeout.InfiniteTimeSpan);

            Container.RegisterInstance(typeof(TokenProvider), _tokenProviderMock.Object);
            Container.RegisterInstance(typeof(HttpService), _httpServiceMock.Object);

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
        public void SendAdditionalRequestIfTokenExpired()
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
                       
            var response = Sut.GetAsync(It.IsAny<string>(),
                It.IsAny<IEnumerable<KeyValuePair<string, string>>>()).Result;

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
            _httpServiceMock.Setup(x
                    => x.GetAsync(
                        It.IsAny<string>(),
                        It.IsAny<IEnumerable<KeyValuePair<string, string>>>()))
                .Callback(()=>
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
