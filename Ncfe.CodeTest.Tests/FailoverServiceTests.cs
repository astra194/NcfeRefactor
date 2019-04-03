using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ncfe.CodeTest.Failover;
using Ncfe.CodeTest.Failover.Repository;
using System;
using System.Collections.Generic;

namespace Ncfe.CodeTest.Tests
{
    [TestClass]
    public class FailoverServiceTests
    {
        private IFailoverRepository _failoverRepository;

        private IFailoverService _sut;

        [TestInitialize]
        public void Setup()
        {
            _failoverRepository = A.Fake<IFailoverRepository>();

            _sut = new FailoverService(_failoverRepository);
        }

        [TestCleanup]
        public void Teardown()
        {
            _failoverRepository = null;
            _sut = null;
        }

        public static IEnumerable<object[]> GetInFailoverModeParameters()
        {
            bool[] all = new bool[] { false, true };

            foreach (var failoverEntriesRecent in all)
            {
                foreach (var failoverEntriesThreshold in all)
                {
                    yield return new object[]
                    {
                          failoverEntriesRecent
                        , failoverEntriesThreshold
                    };
                }
            }
        }

        [DataTestMethod, DynamicData(nameof(GetInFailoverModeParameters), DynamicDataSourceType.Method)]
        public void InFailoverModeTests(
              bool failoverEntriesRecent
            , bool failoverEntriesThreshold
        )
        {
            var failoverEntries = new List<FailoverEntry>(GenerateFailoverEntries(failoverEntriesRecent, failoverEntriesThreshold));
            A.CallTo(() => _failoverRepository.GetFailOverEntries()).Returns(failoverEntries);

            bool expectedResult = failoverEntriesRecent && failoverEntriesThreshold;

            var result = _sut.InFailoverMode();

            Assert.AreEqual(expectedResult, result);
            A.CallTo(() => _failoverRepository.GetFailOverEntries()).MustHaveHappenedOnceExactly();
        }

        private IEnumerable<FailoverEntry> GenerateFailoverEntries(bool failoverEntriesRecent, bool failoverEntriesThreshold)
        {
            int failoverEntriesCount = failoverEntriesThreshold ? 110 : 90;
            for (int i = 0; i < failoverEntriesCount; i++)
            {
                DateTime entryTime = failoverEntriesRecent ? DateTime.Now : DateTime.Now - TimeSpan.FromMinutes(30);
                yield return new FailoverEntry { DateTime = entryTime };
            }
        }
    }
}
