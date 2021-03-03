using NUnit.Framework;
using System;
using SemanticComparison.Fluent;
using ApigeeSDK.Models;
using System.Threading.Tasks;

namespace ApigeeSDK.Unit.Tests
{
    public class GetApplicationByIdShould : ApigeeClientTestsBase
    {
        [Test]
        public async Task ReturnApplicationByIdForValidJson()
        {
            var json = @"{
                              'accessType': 'read',
                              'apiProducts': [],
                              'appFamily': 'default',
                              'appId': '44444444-4444-4444-4444-444444444444',
                              'attributes': [
                                {
                                  'name': 'DisplayName',
                                  'value': 'My Company App'
                                },
                                {
                                  'name': 'creationDate',
                                  'value': '2018-09-04 12:42 PM'
                                },
                                {
                                  'name': 'lastModified',
                                  'value': '2018-09-04 12:42 PM'
                                },
                                {
                                  'name': 'lastModifier',
                                  'value': 'craig.gardener@navico.com'
                                }
                              ],
                              'callbackUrl': 'https://example.com',
                              'companyName': 'craig-gardener-',
                              'createdAt': 1540825200000,
                              'createdBy': 'creator@test.com',
                              'credentials': [
                                {
                                  'apiProducts': [
                                    {
                                      'apiproduct': 'S63-API',
                                      'status': 'approved'
                                    }
                                  ],
                                  'attributes': [],
                                  'consumerKey': 'TpJjTpUm0SjwEtY3MBzeDVrMegOShIHk',
                                  'consumerSecret': '8BNPwsEYVDwNc3f6',
                                  'expiresAt': -1,
                                  'issuedAt': 1536064954059,
                                  'scopes': [],
                                  'status': 'approved'
                                }
                              ],
                              'lastModifiedAt': 1540828800000,
                              'lastModifiedBy': 'test@company.com',
                              'name': 'my-company-app',
                              'scopes': [],
                              'status': 'approved'
                            }".QuotesToDoubleQuotes();

            var applicationId = "44444444-4444-4444-4444-444444444444";

            var expectedApplication = new Application()
            {
                CreatedBy = "creator@test.com",
                ApplicationId = applicationId,
                Status = "approved",
                CreatedAt = 1540825200000,
                LastModifiedBy = "test@company.com",
                LastModifiedAt = 1540828800000,
                Name = "my-company-app"
            };

            var url = BaseUrl + $"/v1/o/{OrgName}/apps/{applicationId}";

            var apigeeService = GetInitializedApigeeService(url, json);
            var application = await apigeeService.GetApplication(expectedApplication.ApplicationId);

            expectedApplication.AsSource().OfLikeness<Application>()
                .Without(x => x.Attributes)
                .Without(x => x.DisplayName)
                .Without(x => x.DeveloperId)
                .Without(x => x.CompanyName)
                .ShouldEqual(application);

            Assert.AreEqual(new DateTime(2018, 10, 29, 15, 0, 0), application.CreatedAtDateTime);
            Assert.AreEqual(new DateTime(2018, 10, 29, 16, 0, 0), application.LastModifiedAtDateTime);

            Assert.AreEqual("My Company App", application.DisplayName);
            Assert.AreEqual("craig-gardener-", application.CompanyName);
            Assert.IsNull(application.DeveloperId);
        }

        [Test]
        public void ThrowJsonSerializationExceptionForInvalidJson()
        {
            var invalidJson = @"[
                    '11111111-1111-1111-1111-111111111
                    '33333333-3333-3333-3333-333333333333'
                ".QuotesToDoubleQuotes();

            var applicationId = "44444444-4444-4444-4444-444444444444";

            var apigeeService = GetInitializedApigeeService(BaseUrl + $"/v1/o/{OrgName}/apps/{applicationId}", invalidJson);

            Assert.ThrowsAsync(Is.InstanceOf<Newtonsoft.Json.JsonException>(), async () =>
                await apigeeService.GetApplication(applicationId));
        }
    }
}
