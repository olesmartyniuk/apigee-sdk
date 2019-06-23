using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity;
using SemanticComparison.Fluent;
using System.Net;
using ApigeeSDK.Models;
using System.Threading.Tasks;

namespace ApigeeSDK.Unit.Tests
{
    public class GetDevelopersShould : ApigeeClientTestsBase
    {
        private const int entitiesLimit = 1000;

        [SetUp]
        protected override void Init()
        {
            base.Init();
            _apigeeClientOptionsMock.Setup(x => x.EntitiesLimit).Returns(entitiesLimit);
        }

        [Test]
        public async Task ReturnListOfDevelopersForValidJson()
        {
            var json = @"{
                ""developer"": [
                    {
                        ""apps"": [],
                        ""companies"": [],
                        ""email"": ""very.secret@company.com"",
                        ""developerId"": ""08842ff2-6512-453a-bb19-b5e4ccc0c0ae"",
                        ""firstName"": ""myFirst"",
                        ""lastName"": ""myLast"",
                        ""userName"": ""username1"",
                        ""organizationName"": ""navico-nonprod"",
                        ""status"": ""active"",
                        ""attributes"": [
                            {
                                ""name"": ""MINT_BILLING_TYPE"",
                                ""value"": ""PREPAID""
                            },
                            {
                                ""name"": ""MINT_DEVELOPER_TYPE"",
                                ""value"": ""UNTRUSTED""
                            }
                        ],
                        ""createdAt"": 1540480731940,
                        ""createdBy"": ""first.man@yes.com"",
                        ""lastModifiedAt"": 1540481731940,
                        ""lastModifiedBy"": ""some.guy2@test.com""
                    },
                    {
                        ""apps"": [
                            ""MPC-App""
                        ],
                        ""companies"": [],
                        ""email"": ""some.Developer@c-c-c.com"",
                        ""developerId"": ""0dc23133-aaf8-46c5-803e-58a412f53bc1"",
                        ""firstName"": ""veryFirstName"",
                        ""lastName"": ""veryLastName"",
                        ""userName"": ""username2"",
                        ""organizationName"": ""navico-nonprod"",
                        ""status"": ""customStatus"",
                        ""attributes"": [
                            {
                                ""name"": ""Comment"",
                                ""value"": ""registered on-behalf: ref. Paul Elgar 28.05.2018""
                            },
                            {
                                ""name"": ""MINT_BILLING_TYPE"",
                                ""value"": ""PREPAID""
                            },
                            {
                                ""name"": ""MINT_DEVELOPER_TYPE"",
                                ""value"": ""UNTRUSTED""
                            }
                        ],
                        ""createdAt"": 1521922839029,
                        ""createdBy"": ""al.al@al.com"",
                        ""lastModifiedAt"": 1531522839029,
                        ""lastModifiedBy"": ""some.guy3@test.com""
                    }
                ]
            }";

            var url = BaseUrl + $"/v1/o/{OrgName}/developers?expand=true&count={entitiesLimit}";

            var apigeeService = GetInitializedApigeeService(url, json);
            var developers = await apigeeService.GetDevelopers();

            Assert.AreEqual(2, developers.Count);

            new Developer()
            {
                LastModifiedBy = "some.guy2@test.com",
                CreatedBy = "first.man@yes.com",
                CreatedAt = 1540480731940,
                LastModifiedAt = 1540481731940,
                Status = "active",
                FirstName = "myFirst",
                LastName = "myLast",
                Email = "very.secret@company.com",
                DeveloperId = "08842ff2-6512-453a-bb19-b5e4ccc0c0ae"
            }.AsSource().OfLikeness<Developer>().ShouldEqual(developers[0]);

            Assert.IsTrue(developers[0].IsActive);

            new Developer()
            {
                LastModifiedBy = "some.guy3@test.com",
                CreatedBy = "al.al@al.com",
                CreatedAt = 1521922839029,
                LastModifiedAt = 1531522839029,
                Status = "customStatus",
                FirstName = "veryFirstName",
                LastName = "veryLastName",
                Email = "some.Developer@c-c-c.com",
                DeveloperId = "0dc23133-aaf8-46c5-803e-58a412f53bc1"
            }.AsSource().OfLikeness<Developer>().ShouldEqual(developers[1]);

            Assert.IsFalse(developers[1].IsActive); 
        }

        [Test]
        public async Task ReturnEmptyListOfDevelopersForEmptyList()
        {
            var json = @"{ ""developer"": [ ] }";
            var url = BaseUrl + $"/v1/o/{OrgName}/developers?expand=true&count={entitiesLimit}";

            var apigeeService = GetInitializedApigeeService(url, json);
            var developers = await apigeeService.GetDevelopers();

            Assert.AreEqual(0, developers.Count);
        }

        [Test]
        public async Task ReturnListOfDevelopersByPortions()
        {
            var jsonPortion1 = @"{
                ""developer"": [
                    {
                        ""email"": ""email1@company.com"",
                        ""developerId"": ""11111111-1111-1111-1111-111111111111""
                    },
                    {
                        ""email"": ""email2@company.com"",
                        ""developerId"": ""22222222-2222-2222-2222-222222222222""
                    },
                    {
                        ""email"": ""email3@company.com"",
                        ""developerId"": ""33333333-3333-3333-3333-333333333333""
                    }
                ]}";

            var jsonPortion2 = @"{
                ""developer"": [
                    {
                        ""email"": ""email3@company.com"",
                        ""developerId"": ""33333333-3333-3333-3333-333333333333""
                    },
                    {
                        ""email"": ""email4@company.com"",
                        ""developerId"": ""44444444-4444-4444-4444-444444444444""
                    }
                ]}";

            var testEntitiesLimit = 3;
            var urlForPortion1 = BaseUrl + $"/v1/o/{OrgName}/developers?expand=true&count={testEntitiesLimit}";
            var urlForPortion2 = BaseUrl + $"/v1/o/{OrgName}/developers?expand=true&count={testEntitiesLimit}&startKey=email3@company.com";

            RegisterUrlAndJson(urlForPortion1, jsonPortion1);
            RegisterUrlAndJson(urlForPortion2, jsonPortion2);
            _apigeeClientOptionsMock.Setup(x => x.EntitiesLimit).Returns(testEntitiesLimit);

            var apigeeService = Container.Resolve<ApigeeClient>();
            var developers = await apigeeService.GetDevelopers();

            Assert.AreEqual(4, developers.Count);

            Assert.AreEqual("email1@company.com", developers[0].Email);
            Assert.AreEqual("11111111-1111-1111-1111-111111111111", developers[0].DeveloperId);

            Assert.AreEqual("email2@company.com", developers[1].Email);
            Assert.AreEqual("22222222-2222-2222-2222-222222222222", developers[1].DeveloperId);

            Assert.AreEqual("email3@company.com", developers[2].Email);
            Assert.AreEqual("33333333-3333-3333-3333-333333333333", developers[2].DeveloperId);

            Assert.AreEqual("email4@company.com", developers[3].Email);
            Assert.AreEqual("44444444-4444-4444-4444-444444444444", developers[3].DeveloperId);
        }

        [Test]
        public void ThrowJsonSerializationExceptionIfJsonIsInvalid()
        {
            var invalidJson = @"[
                    '11111111-1111-1111-1111-111111111
                    '33333333-3333-3333-3333-333333333333'
                ".QuotesToDoubleQuotes();

            var apigeeService = this.GetInitializedApigeeService(
                BaseUrl + $"/v1/o/{OrgName}/developers?expand=true&count={entitiesLimit}",
                invalidJson);

            Assert.ThrowsAsync(Is.InstanceOf<Newtonsoft.Json.JsonException>(), async () =>
                await apigeeService.GetDevelopers());
        }
    }
}
