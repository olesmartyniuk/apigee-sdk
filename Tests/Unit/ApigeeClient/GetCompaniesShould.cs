using System.Threading.Tasks;
using ApigeeSDK.Models;
using SemanticComparison.Fluent;
using Xunit;

namespace ApigeeSDK.Unit.Tests
{
    public class GetCompaniesShould : ApigeeClientTestsBase
    {
        [Fact]
        public async Task ReturnListOfCompaniesForValidJson()
        {
            var json = @"{
                ""company"": [
                    {
                        ""name"": ""test-company-1"",
                        ""displayName"": ""Test Company 1"",
                        ""organization"": ""navico-nonprod"",
                        ""status"": ""active"",
                        ""attributes"": [],
                        ""createdAt"": 1537530386355,
                        ""createdBy"": ""test1@navico.com"",
                        ""lastModifiedAt"": 1537530386356,
                        ""lastModifiedBy"": ""test2@navico.com""
                    },
                    {
                        ""name"": ""test-company-2"",
                        ""displayName"": ""Test Company 2"",
                        ""organization"": ""navico-nprod"",
                        ""status"": ""nonactive"",
                        ""attributes"": [],
                        ""createdAt"": 1537530386357,
                        ""createdBy"": ""test3@navico.com"",
                        ""lastModifiedAt"": 1537530386358,
                        ""lastModifiedBy"": ""test4@navico.com""
                    }
                ]
            }";

            var url = BaseUrl + $"/v1/o/{OrgName}/companies?expand=true&count={EntitiesLimit}";
            RegisterUrl(url, json);
            var apigeeService = GetApigeeClient();
            var companies = await apigeeService.GetCompanies();

            Assert.Equal(2, companies.Count);

            new Company()
            {
                CreatedBy = "test1@navico.com",
                CreatedAt = 1537530386355,
                LastModifiedBy = "test2@navico.com",
                LastModifiedAt = 1537530386356,
                Status = "active",
                Name = "test-company-1",
                DisplayName = "Test Company 1",
                Organization = "navico-nonprod"
            }.AsSource().OfLikeness<Company>().ShouldEqual(companies[0]);

            Assert.True(companies[0].IsActive);

            new Company()
            {
                CreatedBy = "test3@navico.com",
                CreatedAt = 1537530386357,
                LastModifiedBy = "test4@navico.com",
                LastModifiedAt = 1537530386358,
                Status = "nonactive",
                Name = "test-company-2",
                DisplayName = "Test Company 2",
                Organization = "navico-nprod"
            }.AsSource().OfLikeness<Company>().ShouldEqual(companies[1]);

            Assert.False(companies[1].IsActive);
        }

        [Fact]
        public async Task ReturnEmptyListOfCompaniesForEmptyList()
        {
            var json = @"{ ""company"": [ ] }";
            var url = BaseUrl + $"/v1/o/{OrgName}/companies?expand=true&count={EntitiesLimit}";
            RegisterUrl(url, json);
            var apigeeService = GetApigeeClient();
            var companies = await apigeeService.GetCompanies();

            Assert.Equal(0, companies.Count);
        }

        [Fact]
        public async Task ReturnListOfCompaniesByPortions()
        {
            var jsonPortion1 = @"{
                ""company"": [
                    {
                        ""name"": ""company-1"",
                        ""displayName"": ""Company 1""
                    },
                    {
                        ""name"": ""company-2"",
                        ""displayName"": ""Company 2""
                    },
                    {
                        ""name"": ""company-3"",
                        ""displayName"": ""Company 3""
                    }
                ]}";

            var jsonPortion2 = @"{
                ""company"": [
                    {
                        ""name"": ""company-3"",
                        ""displayName"": ""Company 3""
                    },
                    {
                        ""name"": ""company-4"",
                        ""displayName"": ""Company 4""
                    }
                ]}";

            EntitiesLimit = 3;
            var urlForPortion1 = BaseUrl + $"/v1/o/{OrgName}/companies?expand=true&count={EntitiesLimit}";
            var urlForPortion2 = BaseUrl + $"/v1/o/{OrgName}/companies?expand=true&count={EntitiesLimit}&startKey=company-3";

            RegisterUrl(urlForPortion2, jsonPortion2);
            RegisterUrl(urlForPortion1, jsonPortion1);

            var apigeeService = GetApigeeClient();
            var companies = await apigeeService.GetCompanies();

            Assert.Equal(4, companies.Count);

            Assert.Equal("company-1", companies[0].Name);
            Assert.Equal("Company 1", companies[0].DisplayName);

            Assert.Equal("company-2", companies[1].Name);
            Assert.Equal("Company 2", companies[1].DisplayName);

            Assert.Equal("company-3", companies[2].Name);
            Assert.Equal("Company 3", companies[2].DisplayName);

            Assert.Equal("company-4", companies[3].Name);
            Assert.Equal("Company 4", companies[3].DisplayName);
        }

        [Fact]
        public void ThrowJsonSerializationExceptionIfJsonIsInvalid()
        {
            var invalidJson = @"[
                    'Company 1
                    'Company 3'
                ".QuotesToDoubleQuotes();
            RegisterUrl(BaseUrl + $"/v1/o/{OrgName}/companies?expand=true&count={EntitiesLimit}", invalidJson);
            
            var apigeeService = GetApigeeClient();

            Assert.ThrowsAsync<Newtonsoft.Json.JsonException>(async () =>
                await apigeeService.GetCompanies());
        }
    }
}
