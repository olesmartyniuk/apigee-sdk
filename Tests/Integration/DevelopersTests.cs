using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;
using ApigeeSDK.Exceptions;

namespace ApigeeSDK.Integration.Tests
{
    public class DevelopersTests
    {        
        private string _email = Environment.GetEnvironmentVariable("APIGEE_EMAIL");
        private string _password = Environment.GetEnvironmentVariable("APIGEE_PASSWORD");
        private string _orgName = Environment.GetEnvironmentVariable("APIGEE_ORGNAME");
        private string _envName = "test";
        
        [SetUp]
        public void Setup()
        {            
        }

        private ApigeeClient CreateClient()
        {
            var options = new ApigeeClientOptions(
                _email,
                _password,
                _orgName,
                _envName);
            return ApigeeClient.Create(options);
        }

        [Test]
        public async Task GetDevelopersIsSuccessfull()
        {
            var devs = await CreateClient().GetDevelopers();

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
            Assert.That(error.Message, Is.EqualTo("{\"error\":\"unauthorized\",\"error_description\":\"Failed usergrid authentication : error_description - invalid username or password\"}"));
        }

        [Test]
        public async Task GetDevelopersEmailsIsSuccessfull()
        {
            await CreateClient().GetDevelopersEmails();

            Assert.Pass();
        }

        [Test]
        public void GetDevelopersEmailsWithWrongCredentialsRaiseError()
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
            Assert.That(error.Message, Is.EqualTo("{\"error\":\"unauthorized\",\"error_description\":\"Failed usergrid authentication : error_description -invalid username or password\"}"));
        }

        [Test]
        public async Task GetDeveloperByEmailIsSuccessfull()
        {
            var developerExistedByDefault = "helloworld@apigee.com";
            await CreateClient().GetDeveloper(developerExistedByDefault);   

            Assert.Pass(); 
        }

        [Test]
        public void GetDeveloperByEmailWithWrongCredentialsRaiseError()
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
            Assert.That(error.Message, Is.EqualTo("{\"error\":\"unauthorized\",\"error_description\":\"Failed usergrid authentication : error_description -invalid username or password\"}"));
        }

        [Test]
        public void GetDeveloperByWrongEmailRaiseError()
        {
            var developerUndexisted = "WRONG_DEVELOPER_EMAIL";
            
            var error = Assert.ThrowsAsync<ApigeeSDKHttpException>(
                () => CreateClient().GetDeveloper(developerUndexisted));
            
            Assert.That(error.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(error.Message, Is.EqualTo("{\n  \"code\" : \"developer.service.DeveloperIdDoesNotExist\",\n" +
                $"  \"message\" : \"DeveloperId {developerUndexisted} does not exist in organization {_orgName}\",\n  \"contexts\" : [ ]\n}}"));
        }
    }
}