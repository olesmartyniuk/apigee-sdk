using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity;

namespace ApigeeSDK.Unit.Tests
{
    public class GetDevelopersEmailsShould : ApigeeClientTestsBase
    {
        private const int entitiesLimit = 1000;

        [SetUp]
        protected override void Init()
        {
            base.Init();
            _apigeeClientOptionsMock.Setup(x => x.EntitiesLimit).Returns(entitiesLimit);
        }

        [Test]
        public async Task ReturnListOfDeveloperNamesForValidJson()
        {
            var json = @"[
                    'name1',
                    'name2',
                    'name3'
                ]".QuotesToDoubleQuotes();

            var expectedList = new List<string>()
            {
                "name1",
                "name2",
                "name3"
            };

            var url = BaseUrl + $"/v1/o/{OrgName}/developers?count={entitiesLimit}";

            var apigeeService = GetInitializedApigeeService(url, json);
            var developerNames = await apigeeService.GetDevelopersEmails();

            Assert.AreEqual(expectedList.Count, developerNames.Count);
            Assert.AreEqual(expectedList[0], developerNames[0]);
            Assert.AreEqual(expectedList[1], developerNames[1]);
            Assert.AreEqual(expectedList[2], developerNames[2]);
        }

        [Test]
        public async Task ReturnEmptyListOfDeveloperNamesForEmptyJson()
        {
            var json = @"[]";
            var url = BaseUrl + $"/v1/o/{OrgName}/developers?count={entitiesLimit}";

            var apigeeService = GetInitializedApigeeService(url, json);
            var developerNames = await apigeeService.GetDevelopersEmails();

            Assert.AreEqual(0, developerNames.Count);
        }

        [Test]
        public async Task ReturnListOfDeveloperNamesByPortions()
        {
            var jsonPortion1 = @"['name1', 'name2', 'name3' ]".QuotesToDoubleQuotes();
            var jsonPortion2 = @"['name3', 'name4' ]".QuotesToDoubleQuotes();

            var expectedList = new List<string>()
            {
                "name1", "name2", "name3", "name4"
            };

            var testEntitiesLimit = 3;
            var urlForPortion1 = BaseUrl + $"/v1/o/{OrgName}/developers?count={testEntitiesLimit}";
            var urlForPortion2 = BaseUrl + $"/v1/o/{OrgName}/developers?count={testEntitiesLimit}&startKey=name3";

            RegisterUrlAndJson(urlForPortion1, jsonPortion1);
            RegisterUrlAndJson(urlForPortion2, jsonPortion2);
            _apigeeClientOptionsMock.Setup(x => x.EntitiesLimit).Returns(testEntitiesLimit);

            var apigeeService = Container.Resolve<ApigeeClient>();
            var developerNames = await apigeeService.GetDevelopersEmails();

            Assert.AreEqual(expectedList.Count, developerNames.Count);
            Assert.AreEqual(expectedList[0], developerNames[0]);
            Assert.AreEqual(expectedList[1], developerNames[1]);
            Assert.AreEqual(expectedList[2], developerNames[2]);
        }

        [Test]
        public void ThrowJsonSerializationExceptionIfJsonIsInvalid()
        {
            var invalidJson = @"[
                    '11111111-1111-1111-1111-111111111
                    '33333333-3333-3333-3333-333333333333'
                ".QuotesToDoubleQuotes();
            
            var apigeeService = this.GetInitializedApigeeService(
                BaseUrl + $"/v1/o/{OrgName}/developers?count={entitiesLimit}",
                invalidJson);

            Assert.ThrowsAsync(Is.InstanceOf<Newtonsoft.Json.JsonException>(), async () =>
                await apigeeService.GetDevelopersEmails());
        }
    }
}
