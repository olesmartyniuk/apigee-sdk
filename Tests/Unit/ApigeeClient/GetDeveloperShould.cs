using System;
using SemanticComparison.Fluent;
using ApigeeSDK.Models;
using System.Threading.Tasks;
using Xunit;

namespace ApigeeSDK.Unit.Tests
{
    public class GetDeveloperShould : ApigeeClientTestsBase
    {
        [Fact]
        public async Task ReturnDeveloperByEmail()
        {
            var json = @"{
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

            var developerEmail = "api.support@email.com";

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

            var url = BaseUrl + $"/v1/o/{OrgName}/developers/{developerEmail}";
            RegisterUrl(url, json);
            var apigeeService = GetApigeeClient();
            var developer = await apigeeService.GetDeveloper(developerEmail);

            expectedDeveloper.AsSource().OfLikeness<Developer>().ShouldEqual(developer);
                        
            Assert.Equal(true, developer.IsActive);
            Assert.Equal(new DateTime(2018, 10, 29, 15, 0, 0), developer.CreatedAtDateTime);
            Assert.Equal(new DateTime(2018, 10, 29, 16, 0, 0), developer.LastModifiedAtDateTime);
        }

        [Fact]
        public void ThrowJsonSerializationExceptionIfJsonIsInvalid()
        {
            var invalidJson = @"[
                    '11111111-1111-1111-1111-111111111
                    '33333333-3333-3333-3333-333333333333'
                ".QuotesToDoubleQuotes();
            var developerEmail = "developerEmail@email.com";
            RegisterUrl(BaseUrl + $"/v1/o/{OrgName}/developers/{developerEmail}", invalidJson);
            
            var apigeeService = GetApigeeClient();

            Assert.ThrowsAsync<Newtonsoft.Json.JsonException>(async () =>
                await apigeeService.GetDeveloper(developerEmail));
        }
    }
}
