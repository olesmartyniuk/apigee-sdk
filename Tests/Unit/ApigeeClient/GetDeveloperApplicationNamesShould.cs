using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity;

namespace ApigeeSDK.Unit.Tests
{
    public class GetDeveloperApplicationNamesShould : ApigeeClientTestsBase
    {
        private const int entitiesLimit = 1000;

        [SetUp]
        protected override void Init()
        {
            base.Init();
            _apigeeClientOptionsMock.Setup(x => x.EntitiesLimit).Returns(entitiesLimit);
        }

        [Test]
        public async Task ReturnListOfDeveloperApplicationNamesForValidJson()
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


            var developerEmail = "developerEmail@email.com";
            var url = BaseUrl + $"/v1/o/{OrgName}/developers/{developerEmail}/apps?count={entitiesLimit}";

            var apigeeService = GetInitializedApigeeService(url, json);
            var applicationNames = await apigeeService.GetDeveloperApplicationNames(developerEmail);

            Assert.AreEqual(expectedList.Count, applicationNames.Count);
            Assert.AreEqual(expectedList[0], applicationNames[0]);
            Assert.AreEqual(expectedList[1], applicationNames[1]);
            Assert.AreEqual(expectedList[2], applicationNames[2]);
        }

        [Test]
        public async Task ReturnEmptyListOfDeveloperApplicationNamesForEmptyJson()
        {
            var json = @"[]";

            var developerEmail = "developerEmail@email.com";
            var url = BaseUrl + $"/v1/o/{OrgName}/developers/{developerEmail}/apps?count={entitiesLimit}";

            var apigeeService = GetInitializedApigeeService(url, json);
            var applicationNames = await apigeeService.GetDeveloperApplicationNames(developerEmail);

            Assert.AreEqual(0, applicationNames.Count);
        }

        [Test]
        public async Task ReturnListOfDeveloperApplicationNamesByPortions()
        {
            var jsonPortion1 = @"['name1', 'name2', 'name3' ]".QuotesToDoubleQuotes();
            var jsonPortion2 = @"['name3', 'name4' ]".QuotesToDoubleQuotes();
            
            var expectedList = new List<string>()
            {
                "name1", "name2", "name3", "name4"
            };

            var developerEmail = "developerEmail@email.com";
            var testEntitiesLimit = 3;
            var urlForPortion1 = BaseUrl + $"/v1/o/{OrgName}/developers/{developerEmail}/apps?count={testEntitiesLimit}";
            var urlForPortion2 = BaseUrl + $"/v1/o/{OrgName}/developers/{developerEmail}/apps?count={testEntitiesLimit}&startKey=name3";

            RegisterUrlAndJson(urlForPortion1, jsonPortion1);
            RegisterUrlAndJson(urlForPortion2, jsonPortion2);
            _apigeeClientOptionsMock.Setup(x => x.EntitiesLimit).Returns(testEntitiesLimit);

            var apigeeService = Container.Resolve<ApigeeClient>();
            var applicationNames = await apigeeService.GetDeveloperApplicationNames(developerEmail);

            Assert.AreEqual(expectedList.Count, applicationNames.Count);
            Assert.AreEqual(expectedList[0], applicationNames[0]);
            Assert.AreEqual(expectedList[1], applicationNames[1]);
            Assert.AreEqual(expectedList[2], applicationNames[2]);
            Assert.AreEqual(expectedList[3], applicationNames[3]);
        }

        [Test]
        public void ThrowJsonSerializationExceptionIfJsonIsInvalid()
        {
            var invalidJson = @"[
                    '11111111-1111-1111-1111-111111111
                    '33333333-3333-3333-3333-333333333333'
                ".QuotesToDoubleQuotes();

            var developerEmail = "developerEmail@email.com";

            var apigeeService = GetInitializedApigeeService(
                BaseUrl +
                $"/v1/o/{OrgName}/developers/{developerEmail}/apps?count={entitiesLimit}",
                invalidJson);

            Assert.ThrowsAsync(Is.InstanceOf<Newtonsoft.Json.JsonException>(), async () =>
                await apigeeService.GetDeveloperApplicationNames(developerEmail));
        }
    }
}
