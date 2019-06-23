using ApigeeSDK.Models;
using NUnit.Framework;
using SemanticComparison.Fluent;
using System;
using System.Threading.Tasks;

namespace ApigeeSDK.Unit.Tests
{
    public class GetApiProductShould : ApigeeClientTestsBase
    {

        [Test]
        public async Task ReturnApiProductByNameForValidJson()
        {
            var json = @"{
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
                                ""value"": ""[{\""name\"":\""/*/featureinfo/*/*/*/*/*/*.**\"",\""policies\"":{\""request\"":[],\""response\"":[\""{\\\""policyType\\\"":\\\""ExtractVariables\\\"",\\\""async\\\"":false,\\\""continueOnError\\\"":false,\\\""displayName\\\"":\\\""Transaction Policy\\\"",\\\""enabled\\\"":true,\\\""faultRules\\\"":[],\\\""ignoreUnresolvedVariables\\\"":true,\\\""jSONPayload\\\"":{\\\""variable\\\"":[]},\\\""name\\\"":\\\""\\\"",\\\""source\\\"":{\\\""clearPayload\\\"":false,\\\""value\\\"":\\\""response\\\""},\\\""variablePrefix\\\"":\\\""apigee\\\"",\\\""xMLPayload\\\"":{\\\""namespaces\\\"":[],\\\""stopPayloadProcessing\\\"":false,\\\""variable\\\"":[]},\\\""extractions\\\"":[{\\\""Variable\\\"":{\\\""name\\\"":\\\""response.status.code\\\"",\\\""pattern\\\"":[{\\\""ignoreCase\\\"":true,\\\""value\\\"":\\\""{mint.tx.status}\\\""}]}}]}\""]}}]""
                            }
                        ],
                        ""createdAt"": 1540825200000,
                        ""createdBy"": ""test2.name@email.com"",
                        ""description"": ""test description"",
                        ""displayName"": ""Name to display"",
                        ""environments"": [],
                        ""lastModifiedAt"": 1540828800000,
                        ""lastModifiedBy"": ""test.name@email.com"",
                        ""name"": ""Maps Feature Info"",
                        ""proxies"": [
                            ""Maps-Online""
                        ],
                        ""scopes"": [
                            """"
                        ]
                    }";

            var apiProductName = "some product name";

            var expectedApiProduct = new ApiProduct()
            {
                LastModifiedBy = "test.name@email.com",
                CreatedBy = "test2.name@email.com",
                CreatedAt = 1540825200000,
                LastModifiedAt = 1540828800000,
                Description = "test description",
                DisplayName = "Name to display",
                Name = "Maps Feature Info"
            };

            var url = BaseUrl + $"/v1/o/{OrgName}/apiproducts/{apiProductName}";

            var apigeeService = GetInitializedApigeeService(url, json);

            var apiProduct = await apigeeService.GetApiProduct(apiProductName);
            
            expectedApiProduct.AsSource().OfLikeness<ApiProduct>()
                .Without(x => x.Attributes)
                .Without(x => x.IsPublic)
                .ShouldEqual(apiProduct);

            Assert.AreEqual(true, apiProduct.IsPublic);
            Assert.AreEqual(new DateTime(2018, 10, 29, 15, 0, 0), apiProduct.CreatedAtDateTime);
            Assert.AreEqual(new DateTime(2018, 10, 29, 16, 0, 0), apiProduct.LastModifiedAtDateTime);
        }

        [Test]
        public void ThrowJsonSerializationExceptionIfJsonIsInvalid()
        {
            var invalidJson = @"[
                    '11111111-1111-1111-1111-111111111
                    '33333333-3333-3333-3333-333333333333'
                ".QuotesToDoubleQuotes();

            var apiProductName = "some product name";

            var apigeeService = GetInitializedApigeeService(
                BaseUrl + $"/v1/o/{OrgName}/apiproducts/{apiProductName}", invalidJson);

            Assert.ThrowsAsync(Is.InstanceOf<Newtonsoft.Json.JsonException>(), async () =>
                await apigeeService.GetApiProduct(apiProductName));
        }
    }
}
