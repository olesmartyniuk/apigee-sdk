using System;
using System.Net;
using System.Threading.Tasks;
using ApigeeSDK.Exceptions;
using Xunit;

namespace ApigeeSDK.Integration.Tests
{
    public class DevelopersTests
    {        
        private readonly string _email = Environment.GetEnvironmentVariable("APIGEE_EMAIL");
        private readonly string _password = Environment.GetEnvironmentVariable("APIGEE_PASSWORD");
        private readonly string _orgName = Environment.GetEnvironmentVariable("APIGEE_ORGNAME");
        private readonly string _envName = "test";
        
        private ApigeeClient CreateClient()
        {
            var options = new ApigeeClientOptions(
                _email,
                _password,
                _orgName,
                _envName);
            return new ApigeeClient(options);
        }

        [Fact]
        public async Task GetDevelopersIsSuccessfull()
        {
            await CreateClient().GetDevelopers();
        }

        [Fact]
        public async Task GetDevelopersWithWrongCredentialsRaiseError()
        {
            var options = new ApigeeClientOptions(
                "WRONG_USER",
                "WRONG_PASSWORD",
                "WRONG_ORG",
                "WRONG_ENV");
            var client = new ApigeeClient(options);

            var error = await Assert.ThrowsAsync<ApigeeSDKHttpException>(
                () => client.GetApplications());
            
            Assert.Equal(HttpStatusCode.Unauthorized, error.StatusCode);
            Assert.Equal("{\"error\":\"unauthorized\",\"error_description\":\"Invalid Credentials\"}", error.Message);
        }

        [Fact]
        public async Task GetDevelopersEmailsIsSuccessfull()
        {
            await CreateClient().GetDevelopersEmails();
        }

        [Fact]
        public async Task GetDevelopersEmailsWithWrongCredentialsRaiseError()
        {
            var options = new ApigeeClientOptions(
                "WRONG_USER",
                "WRONG_PASSWORD",
                "WRONG_ORG",
                "WRONG_ENV");
            var client = new ApigeeClient(options);

            var error = await Assert.ThrowsAsync<ApigeeSDKHttpException>(
                () => client.GetApplications());
            
            Assert.Equal(HttpStatusCode.Unauthorized, error.StatusCode);
            Assert.Equal("{\"error\":\"unauthorized\",\"error_description\":\"Invalid Credentials\"}", error.Message);
        }

        [Fact]
        public async Task GetDeveloperByEmailIsSuccessfull()
        {
            var developerExistedByDefault = "helloworld@apigee.com";
            await CreateClient().GetDeveloper(developerExistedByDefault);
        }

        [Fact]
        public async Task GetDeveloperByEmailWithWrongCredentialsRaiseError()
        {
            var options = new ApigeeClientOptions(
                "WRONG_USER",
                "WRONG_PASSWORD",
                "WRONG_ORG",
                "WRONG_ENV");
            var client = new ApigeeClient(options);

            var error = await Assert.ThrowsAsync<ApigeeSDKHttpException>(
                () => client.GetApplications());
            
            Assert.Equal(HttpStatusCode.Unauthorized, error.StatusCode);
            Assert.Equal("{\"error\":\"unauthorized\",\"error_description\":\"Invalid Credentials\"}", error.Message);
        }

        [Fact]
        public async Task GetDeveloperByWrongEmailRaiseError()
        {
            var developerUnexisted = "WRONG_DEVELOPER_EMAIL";

            var error = await Assert.ThrowsAsync<ApigeeSDKHttpException>(
                () => CreateClient().GetDeveloper(developerUnexisted));
            
            Assert.Equal(HttpStatusCode.NotFound, error.StatusCode);
            Assert.Equal("{\n  \"code\" : \"developer.service.DeveloperIdDoesNotExist\",\n" +
                $"  \"message\" : \"DeveloperId {developerUnexisted} does not exist in organization {_orgName}\",\n  \"contexts\" : [ ]\n}}", error.Message);
        }
    }
}