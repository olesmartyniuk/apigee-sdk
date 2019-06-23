using ApigeeSDK.Models;
using NUnit.Framework;
using SemanticComparison.Fluent;
using System.Collections.Generic;
using Unity;

namespace ApigeeSDK.Unit.Tests
{
    public class GetApplicationsShould : ApigeeClientTestsBase
    {
        private const int entitiesLimit = 1000;
        private const int statisticsLimit = 14000;

        [SetUp]
        protected override void Init()
        {
            base.Init();
            _apigeeClientOptionsMock.Setup(x => x.EntitiesLimit).Returns(entitiesLimit);
        }

        [Test]
        public void ReturnListOfApplicationsForValidJson()
        {
            string json = @"{
                                ""app"": [
                                    {
                                        ""accessType"": ""read"",
                                        ""apiProducts"": [],
                                        ""appFamily"": ""default"",
                                        ""appId"": ""dd90d065-db71-4e28-afbf-28adfb2384e0"",
                                        ""attributes"": [
                                            {
                                                ""name"": ""DisplayName"",
                                                ""value"": ""My Company App""
                                            },
                                            {
                                                ""name"": ""creationDate"",
                                                ""value"": ""2018-09-04 12:42 PM""
                                            },
                                            {
                                                ""name"": ""lastModified"",
                                                ""value"": ""2018-09-04 12:42 PM""
                                            },
                                            {
                                                ""name"": ""lastModifier"",
                                                ""value"": ""cra@na.com""
                                            }
                                        ],
                                        ""callbackUrl"": ""https://example.com"",
                                        ""companyName"": ""craig-gardener-"",
                                        ""createdAt"": 1536064954015,
                                        ""createdBy"": ""api.pai@api.com"",
                                        ""credentials"": [
                                            {
                                                ""apiProducts"": [
                                                    {
                                                        ""apiproduct"": ""PI"",
                                                        ""status"": ""approved""
                                                    }
                                                ],
                                                ""attributes"": [],
                                                ""consumerKey"": ""TpJjTpUm"",
                                                ""consumerSecret"": ""8BNPws6"",
                                                ""expiresAt"": -1,
                                                ""issuedAt"": 1536064954059,
                                                ""scopes"": [],
                                                ""status"": ""approved""
                                            }
                                        ],
                                        ""lastModifiedAt"": 1536064954016,
                                        ""lastModifiedBy"": ""api.support@api.com"",
                                        ""name"": ""my-company-app"",
                                        ""scopes"": [],
                                        ""status"": ""status1""
                                    },
                                    {
                                        ""apiProducts"": [],
                                        ""appFamily"": ""default"",
                                        ""appId"": ""f564fa70-74c1-44d2-b7f3-a43e385b8021"",
                                        ""attributes"": [
                                            {
                                                ""name"": ""DisplayName"",
                                                ""value"": ""Developer artDocs""
                                            },
                                            {
                                                ""name"": ""Notes"",
                                                ""value"": """"
                                            }
                                        ],
                                        ""callbackUrl"": ""https://developer.j.com"",
                                        ""createdAt"": 1535993897534,
                                        ""createdBy"": ""x.x@x.com"",
                                        ""credentials"": [
                                            {
                                                ""apiProducts"": [
                                                    {
                                                        ""apiproduct"": ""S6PI"",
                                                        ""status"": ""approved""
                                                    },
                                                    {
                                                        ""apiproduct"": ""VADsdard"",
                                                        ""status"": ""approved""
                                                    },
                                                    {
                                                        ""apiproduct"": ""Maps-Online-Standard"",
                                                        ""status"": ""approved""
                                                    }
                                                ],
                                                ""attributes"": [],
                                                ""consumerKey"": ""nXbXfi0igX5xR2s2Z3so83wJ"",
                                                ""consumerSecret"": ""RkXn2ihLD"",
                                                ""expiresAt"": 1535994197613,
                                                ""issuedAt"": 1535993897613,
                                                ""scopes"": [],
                                                ""status"": ""revoked""
                                            },
                                            {
                                                ""apiProducts"": [
                                                    {
                                                        ""apiproduct"": ""S63-API"",
                                                        ""status"": ""approved""
                                                    },
                                                    {
                                                        ""apiproduct"": ""VADs-Online-Standard"",
                                                        ""status"": ""approved""
                                                    },
                                                    {
                                                        ""apiproduct"": ""Maps-Online-Standard"",
                                                        ""status"": ""approved""
                                                    }
                                                ],
                                                ""attributes"": [],
                                                ""consumerKey"": ""xFVj0LOGDWPkn74zpRpDjcDsGbwBSnY4"",
                                                ""consumerSecret"": ""WuE84Zom6aeu14Lm"",
                                                ""expiresAt"": -1,
                                                ""issuedAt"": 1536005188068,
                                                ""scopes"": [],
                                                ""status"": ""approved""
                                            }
                                        ],
                                        ""developerId"": ""c7b4fa3a-8f84-40a5-8b61-48c29b8082d1"",
                                        ""lastModifiedAt"": 1536005188359,
                                        ""lastModifiedBy"": ""rz.rz@rz.com"",
                                        ""name"": ""Developer Portal SmartDocs"",
                                        ""scopes"": [],
                                        ""status"": ""approved""
                                    }
                                ]
                            }";


            string url = BaseUrl + $"/v1/o/{OrgName}/apps?expand=true&rows={entitiesLimit}";

            var apigeeService = this.GetInitializedApigeeService(url, json);

            List<Application> applications = apigeeService.GetApplications().Result;

            Assert.AreEqual(2, applications.Count);

            new Application()
            {
                LastModifiedBy = "api.support@api.com",
                LastModifiedAt = 1536064954016,
                CreatedBy = "api.pai@api.com",
                CreatedAt = 1536064954015,
                Name = "my-company-app",
                Status = "status1",
                ApplicationId = "dd90d065-db71-4e28-afbf-28adfb2384e0"
            }.AsSource().OfLikeness<Application>()
                .Without(x => x.Attributes)
                .Without(x => x.DeveloperId)
                .Without(x => x.DisplayName)
                .Without(x => x.CompanyName)
                .ShouldEqual(applications[0]);

            Assert.AreEqual("My Company App", applications[0].DisplayName);
            Assert.AreEqual("craig-gardener-", applications[0].CompanyName);
            Assert.IsNull(applications[0].DeveloperId);

            new Application()
            {
                LastModifiedBy = "rz.rz@rz.com",
                LastModifiedAt = 1536005188359,
                CreatedBy = "x.x@x.com",
                CreatedAt = 1535993897534,
                Name = "Developer Portal SmartDocs",
                Status = "approved",
                ApplicationId = "f564fa70-74c1-44d2-b7f3-a43e385b8021"
            }.AsSource().OfLikeness<Application>()
                .Without(x => x.Attributes)
                .Without(x => x.DeveloperId)
                .Without(x => x.DisplayName)
                .Without(x => x.CompanyName)
                .ShouldEqual(applications[1]);


            Assert.AreEqual("Developer artDocs", applications[1].DisplayName);
            Assert.AreEqual("c7b4fa3a-8f84-40a5-8b61-48c29b8082d1", applications[1].DeveloperId);
            Assert.IsNull(applications[1].CompanyName);
        }

        [Test]
        public void ReturnEmptyListForEmptyJson()
        {
            string emptyJson = @"{ ""app"": [ ] }";

            string url = BaseUrl + $"/v1/o/{OrgName}/apps?expand=true&rows={entitiesLimit}";

            var apigeeService = this.GetInitializedApigeeService(url, emptyJson);

            List<Application> applications = apigeeService.GetApplications().Result;

            Assert.AreEqual(0, applications.Count);
        }

        [Test]
        public void ReturnListOfApplicationsByPortions()
        {
            string jsonPortion1 = @"{
            ""app"": [
                {
                    ""appId"": ""11111111-1111-1111-1111-111111111111"",
                    ""name"": ""application1""
                },
                {
                    ""appId"": ""22222222-2222-2222-2222-222222222222"",
                    ""name"": ""application2""
                },
                {
                    ""appId"": ""33333333-3333-3333-3333-333333333333"",
                    ""name"": ""application2""
                }
            ]
                            }";

            string jsonPortion2 = @"{
            ""app"": [
                {
                    ""appId"": ""33333333-3333-3333-3333-333333333333"",
                    ""name"": ""application3""
                },
                {
                    ""appId"": ""44444444-4444-4444-4444-444444444444"",
                    ""name"": ""application4""
                }
            ]
                            }";
            int testEntitiesLimit = 3;
            string urlForPortion1 = BaseUrl + $"/v1/o/{OrgName}/apps?expand=true&rows={testEntitiesLimit}";
            string urlForPortion2 = BaseUrl + $"/v1/o/{OrgName}/apps?expand=true&rows={testEntitiesLimit}&startKey=33333333-3333-3333-3333-333333333333";

            this.RegisterUrlAndJson(urlForPortion1, jsonPortion1);
            this.RegisterUrlAndJson(urlForPortion2, jsonPortion2);

            _apigeeClientOptionsMock.Setup(x => x.EntitiesLimit).Returns(testEntitiesLimit);
            var apigeeService = Container.Resolve<ApigeeClient>();

            List<Application> applications = apigeeService.GetApplications().Result;

            Assert.AreEqual(4, applications.Count);

            Assert.AreEqual("application1", applications[0].Name);
            Assert.AreEqual("11111111-1111-1111-1111-111111111111", applications[0].ApplicationId);

            Assert.AreEqual("application2", applications[1].Name);
            Assert.AreEqual("22222222-2222-2222-2222-222222222222", applications[1].ApplicationId);

            Assert.AreEqual("application3", applications[2].Name);
            Assert.AreEqual("33333333-3333-3333-3333-333333333333", applications[2].ApplicationId);

            Assert.AreEqual("application4", applications[3].Name);
            Assert.AreEqual("44444444-4444-4444-4444-444444444444", applications[3].ApplicationId);
        }


        [Test]
        public void ThrowJsonSerializationExceptionForInvalidJson()
        {
            string invalidJson = @"[
                    '11111111-1111-1111-1111-111111111
                    '33333333-3333-3333-3333-333333333333'
                ".QuotesToDoubleQuotes();

            var apigeeService =
                this.GetInitializedApigeeService(
                    BaseUrl + $"/v1/o/{OrgName}/apps?expand=true&rows={entitiesLimit}", invalidJson);

            Assert.ThrowsAsync(Is.InstanceOf<Newtonsoft.Json.JsonException>(), async () =>
                await apigeeService.GetApplications());
        }

    }
}
