using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;
using ApigeeSDK.Exceptions;

namespace ApigeeSDK.Integration.Tests
{
    public class DevelopersTests
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
            return ApigeeClient.Create(options);
        }

        [Test]
        public async Task GetDevelopersIsSuccessfull()
        {
            await CreateClient().GetDevelopers();

            Assert.Pass();
        }

        [Test]
        public void GetDevelopersWithWrongCredentialsRaiseError()
        {
            var options = new ApigeeClientOptions(
                "WRONG_USER",
                "WRONG_PASSWORD",
                "WRONG_ORG",
                "WRONG_ENV");
            var client = ApigeeClient.Create(options);

            var error = Assert.ThrowsAsync<ApigeeSDKHttpException>(
                () => client.GetApplications());
            Assert.That(error.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            Assert.That(error.Message, Is.EqualTo("{\"error\":\"unauthorized\",\"error_description\":\"Bad credentials\"}"));
        }

        [Test]
        public async Task GetDevelopersEmailsIsSuccessfull()
        {
            await CreateClient().GetDevelopersEmails();

            Assert.Pass();
        }
    }
}