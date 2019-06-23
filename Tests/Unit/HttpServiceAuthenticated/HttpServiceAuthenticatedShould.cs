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
        public IUnityContainer Container { get; } = new UnityContainer();
        [Test]
        public void SendAdditionalRequestIfTokenExpired()
        {
            var tokenProviderMock = CreateTokenProvider();
            var httpServiceMock = new Mock<HttpService>(Timeout.InfiniteTimeSpan);

            int numberOfCalls = 0;
            httpServiceMock.Setup(x
                => x.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<KeyValuePair<string, string>>>())).Returns(() =>
            {
                numberOfCalls++;
                return (numberOfCalls == 1)
                    ? Task.FromResult((false, HttpStatusCode.Unauthorized, It.IsAny<string>()))
                    : Task.FromResult((true, HttpStatusCode.Accepted, It.IsAny<string>()));
            });

            Container.RegisterInstance(typeof(TokenProvider), tokenProviderMock.Object);
            Container.RegisterInstance(typeof(HttpService), httpServiceMock.Object);
            Container.RegisterType(typeof(HttpServiceAuthenticated), typeof(HttpServiceAuthenticated));

            var httpServiceAuthenticated = Container.Resolve<HttpServiceAuthenticated>();



            var response = httpServiceAuthenticated.GetAsync(It.IsAny<string>(),
                It.IsAny<IEnumerable<KeyValuePair<string, string>>>()).Result;

            httpServiceMock.Verify(x =>
                x.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<KeyValuePair<string, string>>>()),
                Times.Exactly(2));
            tokenProviderMock.Verify(x => x.GetAuthorizationHeader(false), Times.Once);
            tokenProviderMock.Verify(x => x.GetAuthorizationHeader(true), Times.Once);
        }

        [Test]
        public void RethrowWebExceptionIfBadWebResponse()
        {
            var tokenProviderMock = CreateTokenProvider();
            var httpServiceMock = new Mock<HttpService>(Timeout.InfiniteTimeSpan);

            httpServiceMock.Setup(x
                    => x.GetAsync(
                        It.IsAny<string>(),
                        It.IsAny<IEnumerable<KeyValuePair<string, string>>>()))
                .Returns(Task.FromResult((false, HttpStatusCode.BadRequest, It.IsAny<string>())));


            Container.RegisterInstance(typeof(TokenProvider), tokenProviderMock.Object);
            Container.RegisterInstance(typeof(HttpService), httpServiceMock.Object);
            Container.RegisterType(typeof(HttpServiceAuthenticated), typeof(HttpServiceAuthenticated));

            var httpServiceAuthenticated = Container.Resolve<HttpServiceAuthenticated>();

            Assert.ThrowsAsync<ApigeeSDKHttpException>(async () => await httpServiceAuthenticated.GetAsync(
                It.IsAny<string>(),
                It.IsAny<IEnumerable<KeyValuePair<string, string>>>())
            );
        }

        private Mock<TokenProvider> CreateTokenProvider()
        {
            var tokenMock = new Mock<TokenProvider>();
            tokenMock.Setup(x => x.GetAuthorizationHeader(It.IsAny<bool>())).ReturnsAsync((bool x) =>
            {
                string token = x ? "expiredToken" : "validToken";
                return new KeyValuePair<string, string>("schema", token);
            });
            return tokenMock;
        }
    }
}
