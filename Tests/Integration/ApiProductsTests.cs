using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;
using ApigeeSDK.Exceptions;

namespace ApigeeSDK.Integration.Tests
{
    public class ApiProductsTests
    {        
        [SetUp]
        public void Setup()
        {            
        }

        private ApigeeClient CreateClient()
        {
            var email = Environment.GetEnvironmentVariable("APIGEE_EMAIL");
            var password = Environment.GetEnvironmentVariable("APIGEE_PASSWORD");
            var orgName = Environment.GetEnvironmentVariable("APIGEE_ORGNAME");
            var envName = "test";
            var options = new ApigeeClientOptions(
                email,
                password,
                orgName,
                envName);
            return new ApigeeClient(options);
        }

        [Test]
        public async Task GetApiProductsIsSuccessfull()
        {
            await CreateClient().GetApiProducts();

            Assert.Pass();
        }

        [Test]
        public void GetApiProductsWithWrongCredentialsRaiseError()
        {
            var options = new ApigeeClientOptions(
                "WRONG_USER",
                "WRONG_PASSWORD",
                "WRONG_ORG",
                "WRONG_ENV");
            var client = new ApigeeClient(options);

            var error = Assert.ThrowsAsync<ApigeeSDKHttpException>(
                () => client.GetApiProducts());
            Assert.That(error.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            Assert.That(error.Message, Is.EqualTo("{\"error\":\"unauthorized\",\"error_description\":\"Invalid Credentials\"}"));
        }

        [Test]
        public async Task GetGetApiProductsNamesIsSuccessfull()
        {
            await CreateClient().GetApiProductNames();

            Assert.Pass();
        }

        [Test]
        public async Task GetGetApiProductsWithLimitIsSuccessfull()
        {
            var email = Environment.GetEnvironmentVariable("APIGEE_EMAIL");
            var password = Environment.GetEnvironmentVariable("APIGEE_PASSWORD");
            var orgName = Environment.GetEnvironmentVariable("APIGEE_ORGNAME");
            var envName = "test";
            var options = new ApigeeClientOptions(
                email,
                password,
                orgName,
                envName);
            options.EntitiesLimit = 2;

            var client = new ApigeeClient(options);

            await client.GetApiProducts();

            Assert.Pass();
        }
    }
}