using System.Threading.Tasks;
using ApigeeSDK.Integration.Tests.Utils;
using ApigeeSDK.Models;
using SemanticComparison.Fluent;
using Xunit;

namespace ApigeeSDK.Integration.Tests.ApigeeClient
{
    public class GetApplicationEmailAppNameShould : ApigeeClientTestsBase
    {
        [Fact]
        public async Task ReturnApplicationByNameForDeveloperForValidJson()
        {
            var json = @"{
                        'accessType': 'read',
                        'appFamily': 'default',
                        'appId': '44444444-4444-4444-4444-444444444444',
                        'attributes': [
                            {
                                'name': 'DisplayName',
                                'value': 'CG-TEST'
                            },
                            {
                                'name': 'creationDate',
                                'value': '2018-07-25 13:38 PM'
                            },
                            {
                                'name': 'lastModified',
                                'value': '2018-07-25 13:38 PM'
                            },
                            {
                                'name': 'lastModifier',
                                'value': 'api.support@navico.com'
                            }
                        ],
                        'callbackUrl': 'https://craiggardener.uk',
                        'createdAt': 1532525927154,
                        'createdBy': 'creator@test.com',
                        'credentials': [
                            {
                                'apiProducts': [
                                    {
                                        'apiproduct': 'S63-API',
                                        'status': 'approved'
                                    },
                                    {
                                        'apiproduct': 'VADs-Online-Standard',
                                        'status': 'approved'
                                    },
                                    {
                                        'apiproduct': 'Maps-Online-Internal',
                                        'status': 'approved'
                                    },
                                    {
                                        'apiproduct': 'Maps-Online-Standard',
                                        'status': 'approved'
                                    }
                                ],
                                'attributes': [],
                                'consumerKey': 'VY1ROQnuOR7h0Ns1YPEbvqnqElMcHw5w',
                                'consumerSecret': 'F6nOc4ztS85CO0Nj',
                                'expiresAt': -1,
                                'issuedAt': 1532525927205,
                                'scopes': [],
                                'status': 'approved'
                            }
                        ],
                        'developerId': '5d5dd949-47e2-408e-aaf0-90bad72eb576',
                        'lastModifiedAt': 1532234927154,
                        'lastModifiedBy': 'some.dev@test.com',
                        'name': 'cg-test',
                        'scopes': [],
                        'status': 'approved'
                    }".QuotesToDoubleQuotes();

            var applicationId = "44444444-4444-4444-4444-444444444444";
            var developerEmail = "creator@test.com";
            var applicationName = "cg-test";

            var expectedApplication = new Application()
            {
                CreatedBy = developerEmail,
                ApplicationId = applicationId,
                Status = "approved",
                CreatedAt = 1532525927154,
                LastModifiedBy = "some.dev@test.com",
                LastModifiedAt = 1532234927154,
                Name = applicationName
            };

            var url = BaseUrl + $"/v1/o/{OrgName}/developers/{developerEmail}/apps/{applicationName}";
            RegisterUrl(url, json);
            var apigeeService = GetApigeeClient();
            var application = await apigeeService.GetApplication(developerEmail, applicationName);

            expectedApplication.AsSource().OfLikeness<Application>()
                .Without(x => x.Attributes)
                .Without(x => x.DisplayName)
                .Without(x => x.DeveloperId)
                .Without(x => x.CompanyName)
                .ShouldEqual(application);

            Assert.Equal("CG-TEST", application.DisplayName);
            Assert.Equal("5d5dd949-47e2-408e-aaf0-90bad72eb576", application.DeveloperId);
            Assert.Null(application.CompanyName);
        }

        [Fact]
        public void ThrowJsonSerializationExceptionIfJsonIsInvalid()
        {
            var invalidJson = @"[
                    '11111111-1111-1111-1111-111111111
                    '33333333-3333-3333-3333-333333333333'
                ".QuotesToDoubleQuotes();

            var developerEmail = "developerEmail@email.com";
            var applicationName = "cg-test";

            RegisterUrl(BaseUrl + $"/v1/o/{OrgName}/developers/{developerEmail}/apps/{applicationName}", invalidJson);
            var apigeeService = GetApigeeClient();

            Assert.ThrowsAsync<Newtonsoft.Json.JsonException>(async () =>
                await apigeeService.GetApplication(developerEmail, applicationName));
        }
    }
}
