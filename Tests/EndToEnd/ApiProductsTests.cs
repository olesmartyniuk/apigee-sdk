using System;
using System.Net;
using System.Threading.Tasks;
using ApigeeSDK.Exceptions;
using Xunit;

namespace ApigeeSDK.EndToEnd.Tests
{
    public class ApiProductsTests
    {        
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

        [Fact]
        public async Task GetApiProductsIsSuccessful()
        {
            await CreateClient().GetApiProducts();
        }

        [Fact]
        public async Task GetApiProductsWithWrongCredentialsRaiseError()
        {
            var options = new ApigeeClientOptions(
                "WRONG_USER",
                "WRONG_PASSWORD",
                "WRONG_ORG",
                "WRONG_ENV");
            var client = new ApigeeClient(options);

            var error = await Assert.ThrowsAsync<ApigeeSdkHttpException>(
                () => client.GetApiProducts());
            Assert.Equal(HttpStatusCode.Unauthorized, error.StatusCode);
            Assert.Equal("{\"error\":\"unauthorized\",\"error_description\":\"Invalid Credentials\"}", error.Message);
        }

        [Fact]
        public async Task GetGetApiProductsNamesIsSuccessful()
        {
            await CreateClient().GetApiProductNames();
        }

        [Fact]
        public async Task GetGetApiProductsWithLimitIsSuccessful()
        {
            var email = Environment.GetEnvironmentVariable("APIGEE_EMAIL");
            var password = Environment.GetEnvironmentVariable("APIGEE_PASSWORD");
            var orgName = Environment.GetEnvironmentVariable("APIGEE_ORGNAME");
            var envName = "test";
            var options = new ApigeeClientOptions(
                email,
                password,
                orgName,
                envName) {EntitiesLimit = 2};

            var client = new ApigeeClient(options);

            await client.GetApiProducts();
        }
    }
}