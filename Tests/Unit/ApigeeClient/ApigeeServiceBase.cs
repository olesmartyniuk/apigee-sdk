using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Unity;
using Unity.Injection;
using ApigeeSDK.Services;

namespace ApigeeSDK.Unit.Tests
{
    public class ApigeeClientTestsBase
    {
        protected const string baseUri = "https://api.enterprise.apigee.com";

        protected const string email = "email";
        protected const string password = "password";
        protected const string orgName = "organization";
        protected const string envName = "test";

        protected Mock<HttpServiceAuthenticated> httpServiceAuthenticated;

        protected const int requestTimeOut = 300;

        protected Mock<ApigeeClientOptions> apigeeServiceOptionsMock;

        public IUnityContainer Container { get; } = new UnityContainer();

        [SetUp]
        protected virtual void Init()
        {
            apigeeServiceOptionsMock = new Mock<ApigeeClientOptions>(email, password, orgName, envName);
            apigeeServiceOptionsMock.Setup(x => x.Email)
                .Returns(email);
            apigeeServiceOptionsMock.Setup(x => x.Password)
                .Returns(password);
            apigeeServiceOptionsMock.Setup(x => x.OrgName)
                .Returns(orgName);
            apigeeServiceOptionsMock.Setup(x => x.EnvName)
                .Returns(envName);

            httpServiceAuthenticated = new Mock<HttpServiceAuthenticated>(
                MockBehavior.Strict, 
                new HttpService(TimeSpan.FromSeconds(requestTimeOut)), new Mock<TokenProvider>().Object);
            Container.RegisterInstance(typeof(HttpServiceAuthenticated), httpServiceAuthenticated.Object);
            Container.RegisterInstance(typeof(ApigeeClientOptions), apigeeServiceOptionsMock.Object);
            Container.RegisterType(typeof(ApigeeClient), typeof(ApigeeClient),
                new InjectionConstructor(Container.Resolve<ApigeeClientOptions>(), Container.Resolve<HttpServiceAuthenticated>()));
        }

        protected void RegisterUrlAndJson(string url, string json, HttpStatusCode statusCode = HttpStatusCode.Accepted)
        {
            httpServiceAuthenticated.Setup(x => x.GetAsync(url, It.IsAny<IEnumerable<KeyValuePair<string, string>>>()))
                .Returns(Task.FromResult((true, statusCode, json)));
        }

        protected ApigeeClient GetInitializedApigeeService(string url, string json, HttpStatusCode statusCode = HttpStatusCode.Accepted)
        {
            RegisterUrlAndJson(url, json, statusCode);
            return Container.Resolve<ApigeeClient>();
        }
    }
}
