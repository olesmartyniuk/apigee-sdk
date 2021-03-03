using ApigeeSDK.Models;
using SemanticComparison.Fluent;
using System.Threading.Tasks;
using Xunit;

namespace ApigeeSDK.Unit.Tests
{
    public class GetApiProductsShould : ApigeeClientTestsBase
    {
        [Fact]
        public async Task ReturnListOfApiProductsForValidJson()
        {
            var json = @"{
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

            var url = BaseUrl + $"/v1/o/{OrgName}/apiproducts?expand=true&count={EntitiesLimit}";
            RegisterUrl(url, json);

            var apigeeService = GetApigeeClient();
            var apiProducts = await apigeeService.GetApiProducts();

            Assert.Equal(2, apiProducts.Count);

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

            Assert.True(apiProducts[0].IsPublic);

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

            Assert.False(apiProducts[1].IsPublic);
        }

        [Fact]
        public async Task ReturnListOfApiProductsForEmptyJson()
        {
            var json = @"{ ""apiProduct"": [ ] }";

            var url = BaseUrl + $"/v1/o/{OrgName}/apiproducts?expand=true&count={EntitiesLimit}";
            RegisterUrl(url, json);

            var apigeeService = GetApigeeClient();

            var apiProducts = await apigeeService.GetApiProducts();

            Assert.Empty(apiProducts);
        }

        [Fact]
        public async Task ReturnListOfApiProductsByPortions()
        {
            var jsonPortion1 = @"{
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

            var jsonPortion2 = @"{
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

            EntitiesLimit = 3;

            var urlForPortion1 = BaseUrl + $"/v1/o/{OrgName}/apiproducts?expand=true&count={EntitiesLimit}";
            var urlForPortion2 = BaseUrl + $"/v1/o/{OrgName}/apiproducts?expand=true&count={EntitiesLimit}&startKey=name3";

            RegisterUrl(urlForPortion2, jsonPortion2);
            RegisterUrl(urlForPortion1, jsonPortion1);
            
            var apigeeService = GetApigeeClient();
            var apiProducts = await apigeeService.GetApiProducts();

            Assert.Equal(4, apiProducts.Count);

            Assert.Equal("name1", apiProducts[0].Name);
            Assert.Equal("one.one@one.com", apiProducts[0].CreatedBy);

            Assert.Equal("name2", apiProducts[1].Name);
            Assert.Equal("two.two@two.com", apiProducts[1].CreatedBy);

            Assert.Equal("name3", apiProducts[2].Name);
            Assert.Equal("three.three@three.com", apiProducts[2].CreatedBy);

            Assert.Equal("name4", apiProducts[3].Name);
            Assert.Equal("four.four@four.com", apiProducts[3].CreatedBy);
        }

        [Fact]
        public void ThrowJsonSerializationExceptionIfJsonIsInvalid()
        {
            var invalidJson = @"[
                    '11111111-1111-1111-1111-111111111
                    '33333333-3333-3333-3333-333333333333'
                ".QuotesToDoubleQuotes();
            RegisterUrl(BaseUrl + $"/v1/o/{OrgName}/apiproducts?expand=true&count={EntitiesLimit}", invalidJson);
            
            var apigeeService = GetApigeeClient();
            
            Assert.ThrowsAsync<Newtonsoft.Json.JsonException>(async () =>
                await apigeeService.GetApiProducts());
        }
    }
}
