﻿using NUnit.Framework;
using System.Collections.Generic;
using Unity;

namespace ApigeeSDK.Unit.Tests
{
    class GetApiProductNamesShould : ApigeeClientTestsBase
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
        public void ReturnListOfApiProductsNamesForValidJson()
        {
            string json = @"[
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

            string url = baseUri + $"/v1/o/{orgName}/apiproducts?count={entitiesLimit}";

            var apigeeService = this.GetInitializedApigeeService(url, json);

            List<string> apiProductNames = apigeeService.GetApiProductNames().Result;

            Assert.AreEqual(expectedList.Count, apiProductNames.Count);
            Assert.AreEqual(expectedList[0], apiProductNames[0]);
            Assert.AreEqual(expectedList[1], apiProductNames[1]);
            Assert.AreEqual(expectedList[2], apiProductNames[2]);
        }

        [Test]
        public void ReturnEmptyListOfApiProductsNamesForEmptyJson()
        {
            string json = @"[ ]";

            string url = baseUri + $"/v1/o/{orgName}/apiproducts?count={entitiesLimit}";
            
            var apigeeService = this.GetInitializedApigeeService(url, json);

            List<string> apiProductNames = apigeeService.GetApiProductNames().Result;

            Assert.AreEqual(0, apiProductNames.Count);
        }

        [Test]
        public void ReturnListOfApiProductsNamesByPortions()
        {
            string jsonPortion1 = @"[ 'name1', 'name2', 'name3' ]".QuotesToDoubleQuotes();
            string jsonPortion2 = @"[ 'name3' , 'name4' ]".QuotesToDoubleQuotes();

            var expectedList = new List<string>() { "name1", "name2", "name3", "name4" };

            int testEntitiesLimit = 3;
            string urlForPortion1 = baseUri + $"/v1/o/{orgName}/apiproducts?count={testEntitiesLimit}";
            string urlForPortion2 = baseUri + $"/v1/o/{orgName}/apiproducts?count={testEntitiesLimit}&startKey=name3";

            this.RegisterUrlAndJson(urlForPortion1, jsonPortion1);
            this.RegisterUrlAndJson(urlForPortion2, jsonPortion2);
            apigeeServiceOptionsMock.Setup(x => x.EntitiesLimit).Returns(testEntitiesLimit);
            var apigeeService = Container.Resolve<ApigeeClient>();

            List<string> apiProductNames = apigeeService.GetApiProductNames().Result;

            Assert.AreEqual(expectedList.Count, apiProductNames.Count);
            Assert.AreEqual(expectedList[0], apiProductNames[0]);
            Assert.AreEqual(expectedList[1], apiProductNames[1]);
            Assert.AreEqual(expectedList[2], apiProductNames[2]);
            Assert.AreEqual(expectedList[3], apiProductNames[3]);
        }

        [Test]
        public void ThrowJsonSerializationExceptionIfJsonIsInvalid()
        {
            string invalidJson = @"[
                    '11111111-1111-1111-1111-111111111
                    '33333333-3333-3333-3333-333333333333'
                ".QuotesToDoubleQuotes();

            var apigeeService = this.GetInitializedApigeeService(
                baseUri + $"/v1/o/{orgName}/apiproducts?count={entitiesLimit}",
                invalidJson);

            Assert.ThrowsAsync(Is.InstanceOf<Newtonsoft.Json.JsonException>(), async () =>
                await apigeeService.GetApiProductNames());
        }
    }
}