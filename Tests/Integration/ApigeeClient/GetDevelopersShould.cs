using System.Threading.Tasks;
using ApigeeSDK.Integration.Tests.Utils;
using ApigeeSDK.Models;
using SemanticComparison.Fluent;
using Xunit;

namespace ApigeeSDK.Integration.Tests.ApigeeClient
{
    public class GetDevelopersShould : ApigeeClientTestsBase
    {
        [Fact]
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

            var url = BaseUrl + $"/v1/o/{OrgName}/developers?expand=true&count={EntitiesLimit}";
            RegisterUrl(url, json);
            var apigeeService = GetApigeeClient();
            var developers = await apigeeService.GetDevelopers();

            Assert.Equal(2, developers.Count);

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

            Assert.True(developers[0].IsActive);

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

            Assert.False(developers[1].IsActive); 
        }

        [Fact]
        public async Task ReturnEmptyListOfDevelopersForEmptyList()
        {
            var json = @"{ ""developer"": [ ] }";
            var url = BaseUrl + $"/v1/o/{OrgName}/developers?expand=true&count={EntitiesLimit}";
            RegisterUrl(url, json);

            var apigeeService = GetApigeeClient();
            var developers = await apigeeService.GetDevelopers();

            Assert.Empty(developers);
        }

        [Fact]
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

            EntitiesLimit = 3;
            var urlForPortion1 = BaseUrl + $"/v1/o/{OrgName}/developers?expand=true&count={EntitiesLimit}";
            var urlForPortion2 = BaseUrl + $"/v1/o/{OrgName}/developers?expand=true&count={EntitiesLimit}&startKey=email3@company.com";

            RegisterUrl(urlForPortion2, jsonPortion2);
            RegisterUrl(urlForPortion1, jsonPortion1);

            var apigeeService = GetApigeeClient();
            var developers = await apigeeService.GetDevelopers();

            Assert.Equal(4, developers.Count);

            Assert.Equal("email1@company.com", developers[0].Email);
            Assert.Equal("11111111-1111-1111-1111-111111111111", developers[0].DeveloperId);

            Assert.Equal("email2@company.com", developers[1].Email);
            Assert.Equal("22222222-2222-2222-2222-222222222222", developers[1].DeveloperId);

            Assert.Equal("email3@company.com", developers[2].Email);
            Assert.Equal("33333333-3333-3333-3333-333333333333", developers[2].DeveloperId);

            Assert.Equal("email4@company.com", developers[3].Email);
            Assert.Equal("44444444-4444-4444-4444-444444444444", developers[3].DeveloperId);
        }

        [Fact]
        public void ThrowJsonSerializationExceptionIfJsonIsInvalid()
        {
            var invalidJson = @"[
                    '11111111-1111-1111-1111-111111111
                    '33333333-3333-3333-3333-333333333333'
                ".QuotesToDoubleQuotes();
            RegisterUrl(BaseUrl + $"/v1/o/{OrgName}/developers?expand=true&count={EntitiesLimit}", invalidJson);
            var apigeeService = GetApigeeClient();

            Assert.ThrowsAsync<Newtonsoft.Json.JsonException>(async () =>
                await apigeeService.GetDevelopers());
        }
    }
}
