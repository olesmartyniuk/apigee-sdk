using ApigeeSDK.Models;
using NUnit.Framework;
using SemanticComparison.Fluent;
using System;
using System.Collections.Generic;
using Unity;

namespace ApigeeSDK.Unit.Tests
{
    class GetApiProductsShould : ApigeeClientTestsBase
    {
        private const int entitiesLimit = 1000;
        private const int statisticsLimit = 14000;

        [SetUp]
        protected override void Init()
        {
            base.Init();
            apigeeServiceOptionsMock.Setup(x => x.EntitiesLimit).Returns(entitiesLimit);            
        }


        [Test]
        public void ReturnListOfApiProductsForValidJson()
        {
            string json = @"{
                              ""apiProduct"": [
                                {
                                  ""apiResources"": [
                                    ""/*/featureinfo/*/*/*/*/*/*.**""
                                  ],
                                  ""approvalType"": ""auto"",
                                  ""attributes"": [
                                    {
                                      ""name"": ""MINT_TRANSACTION_SUCCESS_CRITERIA"",
                                      ""value"": ""txProviderStatus == '200'""
                                    },
                                    {
                                      ""name"": ""access"",
                                      ""value"": ""public""
                                    },
                                    {
                                      ""name"": ""transactionRecordingPolicies"",
                                      ""value"": ""[
                              {\""name\"":\""/*/featureinfo/*/*/*/*/*/*.**\"",\""policies\"":{\""request\"":[],\""response\"":[\""{\\\""policyType\\\"":\\\""ExtractVariables\\\"",\\\""async\\\"":false,\\\""continueOnError\\\"":false,\\\""displayName\\\"":\\\""Transaction Policy\\\"",\\\""enabled\\\"":true,\\\""faultRules\\\"":[],\\\""ignoreUnresolvedVariables\\\"":true,\\\""jSONPayload\\\"":{\\\""variable\\\"":[]},\\\""name\\\"":\\\""\\\"",\\\""source\\\"":{\\\""clearPayload\\\"":false,\\\""value\\\"":\\\""response\\\""},\\\""variablePrefix\\\"":\\\""apigee\\\"",\\\""xMLPayload\\\"":{\\\""namespaces\\\"":[],\\\""stopPayloadProcessing\\\"":false,\\\""variable\\\"":[]},\\\""extractions\\\"":[{\\\""Variable\\\"":{\\\""name\\\"":\\\""response.status.code\\\"",\\\""pattern\\\"":[{\\\""ignoreCase\\\"":true,\\\""value\\\"":\\\""{mint.tx.status}\\\""}]}}]}\""]}}]""
                                    }
                                  ],
                                  ""createdAt"": 1536070064743,
                                  ""createdBy"": ""jerry.jerry@jerry.com"",
                                  ""description"": ""some description"",
                                  ""displayName"": ""name to Display"",
                                  ""environments"": [],
                                  ""lastModifiedAt"": 1536070444283,
                                  ""lastModifiedBy"": ""tom.tom@tom.com"",
                                  ""name"": ""Some name"",
                                  ""proxies"": [
                                    ""Maps-Online""
                                  ],
                                  ""scopes"": [
                                    """"
                                  ]
                                },
                                {
                                  ""apiResources"": [
                                    ""/"",
                                    ""/**""
                                  ],
                                  ""approvalType"": ""auto"",
                                  ""attributes"": [
                                    {
                                      ""name"": ""MINT_CUSTOM_ATTRIBUTE_1"",
                                      ""value"": ""weight""
                                    },
                                    {
                                      ""name"": ""MINT_TRANSACTION_SUCCESS_CRITERIA"",
                                      ""value"": ""txProviderStatus == 'OK'""
                                    },
                                    {
                                      ""name"": ""transactionRecordingPolicies"",
                                      ""value"": ""[{\""name\"":\""/\"",\""policies\"":{\""request\"":[],\""response\"":[\""{\\\""policyType\\\"":\\\""ExtractVariables\\\"",\\\""async\\\"":false,\\\""continueOnError\\\"":false,\\\""displayName\\\"":\\\""Transaction Policy\\\"",\\\""enabled\\\"":true,\\\""faultRules\\\"":[],\\\""ignoreUnresolvedVariables\\\"":true,\\\""jSONPayload\\\"":{\\\""variable\\\"":[]},\\\""name\\\"":\\\""\\\"",\\\""source\\\"":{\\\""clearPayload\\\"":false,\\\""value\\\"":\\\""response\\\""},\\\""variablePrefix\\\"":\\\""apigee\\\"",\\\""xMLPayload\\\"":{\\\""namespaces\\\"":[],\\\""stopPayloadProcessing\\\"":false,\\\""variable\\\"":[]},\\\""extractions\\\"":[{\\\""Variable\\\"":{\\\""name\\\"":\\\""response.reason.phrase\\\"",\\\""pattern\\\"":[{\\\""ignoreCase\\\"":true,\\\""value\\\"":\\\""{mint.tx.status}\\\""}]}},{\\\""Header\\\"":{\\\""name\\\"":\\\""weight\\\"",\\\""pattern\\\"":[{\\\""ignoreCase\\\"":true,\\\""value\\\"":\\\""{mint.tx.cust_att1}\\\""}]}}]}\""]}},{\""name\"":\""/**\"",\""policies\"":{\""request\"":[],\""response\"":[\""{\\\""policyType\\\"":\\\""ExtractVariables\\\"",\\\""async\\\"":false,\\\""continueOnError\\\"":false,\\\""displayName\\\"":\\\""Transaction Policy\\\"",\\\""enabled\\\"":true,\\\""faultRules\\\"":[],\\\""ignoreUnresolvedVariables\\\"":true,\\\""jSONPayload\\\"":{\\\""variable\\\"":[]},\\\""name\\\"":\\\""\\\"",\\\""source\\\"":{\\\""clearPayload\\\"":false,\\\""value\\\"":\\\""response\\\""},\\\""variablePrefix\\\"":\\\""apigee\\\"",\\\""xMLPayload\\\"":{\\\""namespaces\\\"":[],\\\""stopPayloadProcessing\\\"":false,\\\""variable\\\"":[]},\\\""extractions\\\"":[{\\\""Variable\\\"":{\\\""name\\\"":\\\""response.reason.phrase\\\"",\\\""pattern\\\"":[{\\\""ignoreCase\\\"":true,\\\""value\\\"":\\\""{mint.tx.status}\\\""}]}},{\\\""Header\\\"":{\\\""name\\\"":\\\""weight\\\"",\\\""pattern\\\"":[{\\\""ignoreCase\\\"":true,\\\""value\\\"":\\\""{mint.tx.cust_att1}\\\""}]}}]}\""]}}]""
                                    }
                                  ],
                                  ""createdAt"": 1531931088872,
                                  ""createdBy"": ""one.two@three.com"",
                                  ""description"": ""Some very long description about product can be inserted here to check if long string can be parsed correctly.Some very long description about product can be inserted here to check if long string can be parsed correctly."",
                                  ""displayName"": ""some name 3"",
                                  ""environments"": [
                                    ""dev"",
                                    ""test""
                                  ],
                                  ""lastModifiedAt"": 1538584334657,
                                  ""lastModifiedBy"": ""custom@one.com"",
                                  ""name"": ""some name2"",
                                  ""proxies"": [
                                    ""Maps-Online""
                                  ],
                                  ""quota"": ""{entitiesLimit}0"",
                                  ""quotaInterval"": ""1"",
                                  ""quotaTimeUnit"": ""hour"",
                                  ""scopes"": [
                                    """"
                                  ]
                                }
                              ]
                            }";

            string url = baseUri + $"/v1/o/{orgName}/apiproducts?expand=true&count={entitiesLimit}";

            var apigeeService = this.GetInitializedApigeeService(url, json);

            List<ApiProduct> apiProducts = apigeeService.GetApiProducts().Result;

            Assert.AreEqual(2, apiProducts.Count);

            new ApiProduct()
            {
                LastModifiedBy = "tom.tom@tom.com",
                LastModifiedAt = 1536070444283,
                CreatedBy = "jerry.jerry@jerry.com",
                CreatedAt = 1536070064743,
                Description = "some description",
                Name = "Some name",
                DisplayName = "name to Display"
            }.AsSource().OfLikeness<ApiProduct>()
                .Without(x => x.Attributes)
                .Without(x => x.IsPublic)
                .ShouldEqual(apiProducts[0]);

            Assert.IsTrue(apiProducts[0].IsPublic);


            new ApiProduct()
            {
                LastModifiedBy = "custom@one.com",
                LastModifiedAt = 1538584334657,
                CreatedBy = "one.two@three.com",
                CreatedAt = 1531931088872,
                Description = "Some very long description about product can be inserted here to check if long string can be parsed correctly.Some very long description about product can be inserted here to check if long string can be parsed correctly.",
                Name = "some name2",
                DisplayName = "some name 3"

            }.AsSource().OfLikeness<ApiProduct>()
                .Without(x => x.Attributes)
                .Without(x => x.IsPublic)
                .ShouldEqual(apiProducts[1]);

            Assert.IsFalse(apiProducts[1].IsPublic);

        }

        [Test]
        public void ReturnListOfApiProductsForEmptyJson()
        {
            string json = @"{ ""apiProduct"": [ ] }";

            string url = baseUri + $"/v1/o/{orgName}/apiproducts?expand=true&count={entitiesLimit}";

            var apigeeService = this.GetInitializedApigeeService(url, json);

            List<ApiProduct> apiProducts = apigeeService.GetApiProducts().Result;

            Assert.AreEqual(0, apiProducts.Count);
        }

        [Test]
        public void ReturnListOfApiProductsByPortions()
        {
            string jsonPortion1 = @"{
                              ""apiProduct"": [
                                {
                                  ""createdBy"": ""one.one@one.com"",
                                  ""name"": ""name1""
                                },
                                {
                                  ""createdBy"": ""two.two@two.com"",
                                  ""name"": ""name2"",
                                },
                                {
                                  ""createdBy"": ""three.three@three.com"",
                                  ""name"": ""name3""
                                }
                              ]
                            }";

            string jsonPortion2 = @"{
                              ""apiProduct"": [
                                {
                                  ""createdBy"": ""three.three@three.com"",
                                  ""name"": ""name3""
                                },
                                {
                                  ""createdBy"": ""four.four@four.com"",
                                  ""name"": ""name4"",
                                }
                              ]
                            }";

            int testEntitiesLimit = 3;

            string urlForPortion1 = baseUri + $"/v1/o/{orgName}/apiproducts?expand=true&count={testEntitiesLimit}";
            string urlForPortion2 = baseUri + $"/v1/o/{orgName}/apiproducts?expand=true&count={testEntitiesLimit}&startKey=name3";

            this.RegisterUrlAndJson(urlForPortion1, jsonPortion1);
            this.RegisterUrlAndJson(urlForPortion2, jsonPortion2);
            apigeeServiceOptionsMock.Setup(x => x.EntitiesLimit).Returns(testEntitiesLimit);
            var apigeeService = Container.Resolve<ApigeeClient>();

            List<ApiProduct> apiProducts = apigeeService.GetApiProducts().Result;

            Assert.AreEqual(4, apiProducts.Count);

            Assert.AreEqual("name1", apiProducts[0].Name);
            Assert.AreEqual("one.one@one.com", apiProducts[0].CreatedBy);

            Assert.AreEqual("name2", apiProducts[1].Name);
            Assert.AreEqual("two.two@two.com", apiProducts[1].CreatedBy);

            Assert.AreEqual("name3", apiProducts[2].Name);
            Assert.AreEqual("three.three@three.com", apiProducts[2].CreatedBy);

            Assert.AreEqual("name4", apiProducts[3].Name);
            Assert.AreEqual("four.four@four.com", apiProducts[3].CreatedBy);

        }

        [Test]
        public void ThrowJsonSerializationExceptionIfJsonIsInvalid()
        {
            string invalidJson = @"[
                    '11111111-1111-1111-1111-111111111
                    '33333333-3333-3333-3333-333333333333'
                ".QuotesToDoubleQuotes();

            var apigeeService = this.GetInitializedApigeeService(
                baseUri + $"/v1/o/{orgName}/apiproducts?expand=true&count={entitiesLimit}", invalidJson);
            

            Assert.ThrowsAsync(Is.InstanceOf<Newtonsoft.Json.JsonException>(), async () =>
                await apigeeService.GetApiProducts());
        }

    }
}
