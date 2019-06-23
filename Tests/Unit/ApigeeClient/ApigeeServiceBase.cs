using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Unity;
using Unity.Injection;
using ApigeeSDK.Services;
using ApigeeSDK.Exceptions;

namespace ApigeeSDK.Unit.Tests
{
    public class ApigeeClientTestsBase
    {
        protected const string BaseUrl = "https://api.enterprise.apigee.com";
        protected const string Email = "email";
        protected const string Password = "password";
        protected const string OrgName = "organization";
        protected const string EnvName = "test";
        protected const int RequestTimeOut = 300;

        protected Mock<ApigeeClientOptions> _apigeeClientOptionsMock;
        protected Mock<HttpService> _httpServiceMock;
        protected Mock<TokenProvider> _tokenProviderMock;
        protected Mock<HttpServiceAuthenticated> _httpServiceAuthenticatedMock;
        
        public IUnityContainer Container { get; } = new UnityContainer();

        [SetUp]
        protected virtual void Init()
        {
            _apigeeClientOptionsMock = new Mock<ApigeeClientOptions>(Email, Password, OrgName, EnvName);
            _apigeeClientOptionsMock.Setup(x => x.Email)
                .Returns(Email);
            _apigeeClientOptionsMock.Setup(x => x.Password)
                .Returns(Password);
            _apigeeClientOptionsMock.Setup(x => x.OrgName)
                .Returns(OrgName);
            _apigeeClientOptionsMock.Setup(x => x.EnvName)
                .Returns(EnvName);
            _apigeeClientOptionsMock.Setup(x => x.BaseUrl)
                .Returns(BaseUrl);
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

            _httpServiceAuthenticatedMock = new Mock<HttpServiceAuthenticated>(
                MockBehavior.Strict, _httpServiceMock.Object, _tokenProviderMock.Object);
            Container.RegisterInstance(_httpServiceAuthenticatedMock.Object);

            Container.RegisterSingleton<ApigeeClient>();
        }

        protected void RegisterUrlAndJson(string url, string json, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            if (statusCode == HttpStatusCode.OK)
            {
                _httpServiceAuthenticatedMock.Setup(x => x.GetAsync(url, It.IsAny<IEnumerable<KeyValuePair<string, string>>>()))
                    .Returns(Task.FromResult(json));
            }
            else
            {
                _httpServiceAuthenticatedMock.Setup(x => x.GetAsync(url, It.IsAny<IEnumerable<KeyValuePair<string, string>>>()))
                    .Callback(() =>
                    {
                        throw new ApigeeSDKHttpException(statusCode, statusCode.ToString());
                    });
            }
        }

        protected ApigeeClient GetInitializedApigeeService(string url, string json, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            RegisterUrlAndJson(url, json, statusCode);
            return Container.Resolve<ApigeeClient>();
        }
    }
}