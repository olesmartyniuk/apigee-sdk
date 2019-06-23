using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ApigeeSDK.Models;
using NUnit.Framework;
using SemanticComparison.Fluent;
using Unity;

namespace ApigeeSDK.Unit.Tests
{
    public class GetCompaniesShould : ApigeeClientTestsBase
    {
        private const int entitiesLimit = 1000;

        [SetUp]
        protected override void Init()
        {
            base.Init();
            _apigeeClientOptionsMock.Setup(x => x.EntitiesLimit).Returns(entitiesLimit);
        }

        [Test]
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

            var url = BaseUrl + $"/v1/o/{OrgName}/companies?expand=true&count={entitiesLimit}";

            var apigeeService = GetInitializedApigeeService(url, json);
            var companies = await apigeeService.GetCompanies();

            Assert.AreEqual(2, companies.Count);

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

            Assert.IsTrue(companies[0].IsActive);

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

            Assert.IsFalse(companies[1].IsActive);
        }

        [Test]
        public async Task ReturnEmptyListOfCompaniesForEmptyList()
        {
            var json = @"{ ""company"": [ ] }";
            var url = BaseUrl + $"/v1/o/{OrgName}/companies?expand=true&count={entitiesLimit}";

            var apigeeService = GetInitializedApigeeService(url, json);
            var companies = await apigeeService.GetCompanies();

            Assert.AreEqual(0, companies.Count);
        }

        [Test]
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

            var testEntitiesLimit = 3;
            var urlForPortion1 = BaseUrl + $"/v1/o/{OrgName}/companies?expand=true&count={testEntitiesLimit}";
            var urlForPortion2 = BaseUrl + $"/v1/o/{OrgName}/companies?expand=true&count={testEntitiesLimit}&startKey=company-3";

            RegisterUrlAndJson(urlForPortion1, jsonPortion1);
            RegisterUrlAndJson(urlForPortion2, jsonPortion2);
            _apigeeClientOptionsMock.Setup(x => x.EntitiesLimit).Returns(testEntitiesLimit);

            var apigeeService = Container.Resolve<ApigeeClient>();
            var companies = await apigeeService.GetCompanies();

            Assert.AreEqual(4, companies.Count);

            Assert.AreEqual("company-1", companies[0].Name);
            Assert.AreEqual("Company 1", companies[0].DisplayName);

            Assert.AreEqual("company-2", companies[1].Name);
            Assert.AreEqual("Company 2", companies[1].DisplayName);

            Assert.AreEqual("company-3", companies[2].Name);
            Assert.AreEqual("Company 3", companies[2].DisplayName);

            Assert.AreEqual("company-4", companies[3].Name);
            Assert.AreEqual("Company 4", companies[3].DisplayName);
        }

        [Test]
        public void ThrowJsonSerializationExceptionIfJsonIsInvalid()
        {
            var invalidJson = @"[
                    'Company 1
                    'Company 3'
                ".QuotesToDoubleQuotes();

            var apigeeService = this.GetInitializedApigeeService(
                BaseUrl + $"/v1/o/{OrgName}/companies?expand=true&count={entitiesLimit}",
                invalidJson);

            Assert.ThrowsAsync(Is.InstanceOf<Newtonsoft.Json.JsonException>(), async () =>
                await apigeeService.GetCompanies());
        }
    }
}
