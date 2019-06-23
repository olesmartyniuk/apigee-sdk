using NUnit.Framework;
using System;
using SemanticComparison.Fluent;
using ApigeeSDK.Models;

namespace ApigeeSDK.Unit.Tests
{
    public class GetDeveloperShould : ApigeeClientTestsBase
    {
        [Test]
        public void ReturnDeveloperByEmail()
        {
            string json = @"{
                                'apps': [
                                    'cg-test'
                                ],
                                'companies': [],
                                'email': 'api.support@email.com',
                                'developerId': '44444444-4444-4444-4444-444444444444',
                                'firstName': 'FName',
                                'lastName': 'LName',
                                'userName': 'navico-webmaster',
                                'organizationName': 'navico-nonprod',
                                'status': 'active',
                                'attributes': [
                                    {
                                        'name': 'MINT_BILLING_TYPE',
                                        'value': 'PREPAID'
                                    },
                                    {
                                        'name': 'MINT_DEVELOPER_TYPE',
                                        'value': 'UNTRUSTED'
                                    }
                                ],
                                'createdAt': 1540825200000,
                                'createdBy': 'test2.name@email.com',
                                'lastModifiedAt': 1540828800000,
                                'lastModifiedBy': 'test.name@email.com'
                            }".QuotesToDoubleQuotes();

            string developerEmail = "api.support@email.com";

            var expectedDeveloper = new Developer()
            {
                LastModifiedBy = "test.name@email.com",
                CreatedBy = "test2.name@email.com",
                CreatedAt = 1540825200000,
                Status = "active",
                LastModifiedAt = 1540828800000,
                DeveloperId = "44444444-4444-4444-4444-444444444444",
                Email = developerEmail,
                FirstName = "FName",
                LastName = "LName"
            };

            string url = BaseUrl + $"/v1/o/{OrgName}/developers/{developerEmail}";


            var apigeeService = this.GetInitializedApigeeService(url, json);

            Developer developer = apigeeService.GetDeveloper(developerEmail).Result;

            expectedDeveloper.AsSource().OfLikeness<Developer>().ShouldEqual(developer);

            
            Assert.AreEqual(true, developer.IsActive);
            Assert.AreEqual(new DateTime(2018, 10, 29, 15, 0, 0), developer.CreatedAtDateTime);
            Assert.AreEqual(new DateTime(2018, 10, 29, 16, 0, 0), developer.LastModifiedAtDateTime);
        }

        [Test]
        public void ThrowJsonSerializationExceptionIfJsonIsInvalid()
        {
            string invalidJson = @"[
                    '11111111-1111-1111-1111-111111111
                    '33333333-3333-3333-3333-333333333333'
                ".QuotesToDoubleQuotes();

            Guid applicationId = new Guid("44444444-4444-4444-4444-444444444444");
            string developerEmail = "developerEmail@email.com";

            var apigeeService = this.GetInitializedApigeeService(BaseUrl + $"/v1/o/{OrgName}/developers/{developerEmail}", invalidJson);

            Assert.ThrowsAsync(Is.InstanceOf<Newtonsoft.Json.JsonException>(), async () =>
                await apigeeService.GetDeveloper(developerEmail));
        }
    }
}
