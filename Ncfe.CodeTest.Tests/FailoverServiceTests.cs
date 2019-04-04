using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ncfe.CodeTest.Configuration;
using Ncfe.CodeTest.Failover;
using Ncfe.CodeTest.Failover.Repository;
using System;
using System.Collections.Generic;

namespace Ncfe.CodeTest.Tests
{
    [TestClass]
    public class FailoverServiceTests
    {
        private readonly TimeSpan _recentPeriod = TimeSpan.FromMinutes(10);
        private const int _failoverThreshold = 100;

        private IConfiguration _configuration;
        private IFailoverRepository _failoverRepository;

        private IFailoverService _sut;

        [TestInitialize]
        public void Setup()
        {
            _configuration = A.Fake<IConfiguration>();
            A.CallTo(() => _configuration.RecentFailoverPeriod).Returns(_recentPeriod);
            A.CallTo(() => _configuration.FailoverRecordsCountThreshold).Returns(_failoverThreshold);
            _failoverRepository = A.Fake<IFailoverRepository>();

            _sut = new FailoverService(_failoverRepository, _configuration);
        }

        [TestCleanup]
        public void Teardown()
        {
            _configuration = null;
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
                    foreach (var failoverModeEnabled in all)
                    {
                        yield return new object[]
                        {
                              failoverEntriesRecent
                            , failoverEntriesThreshold
                            , failoverModeEnabled
                        };
                    }
                }
            }
        }

        [DataTestMethod, DynamicData(nameof(GetInFailoverModeParameters), DynamicDataSourceType.Method)]
        public void InFailoverModeTests(
              bool failoverEntriesRecent
            , bool failoverEntriesThreshold
            , bool failoverModeEnabled
        )
        {
            var failoverEntries = new List<FailoverEntry>(GenerateFailoverEntries(failoverEntriesRecent, failoverEntriesThreshold));
            A.CallTo(() => _failoverRepository.GetFailOverEntries()).Returns(failoverEntries);

            A.CallTo(() => _configuration.IsFailoverModeEnabled).Returns(failoverModeEnabled);

            bool expectedResult = failoverEntriesRecent && failoverEntriesThreshold && failoverModeEnabled;

            var result = _sut.InFailoverMode();

            Assert.AreEqual(expectedResult, result);

            if (failoverModeEnabled)
                A.CallTo(() => _failoverRepository.GetFailOverEntries()).MustHaveHappenedOnceExactly();
            else
                A.CallTo(() => _failoverRepository.GetFailOverEntries()).MustNotHaveHappened();
        }

        private IEnumerable<FailoverEntry> GenerateFailoverEntries(bool failoverEntriesRecent, bool failoverEntriesThreshold)
        {
            int failoverEntriesCount = failoverEntriesThreshold ? _failoverThreshold : _failoverThreshold - 1;
            for (int i = 0; i < failoverEntriesCount; i++)
            {
                DateTime entryTime = failoverEntriesRecent ? DateTime.Now - _recentPeriod + TimeSpan.FromMinutes(1) : DateTime.Now - _recentPeriod - TimeSpan.FromMinutes(1);
                yield return new FailoverEntry { DateTime = entryTime };
            }
        }
    }
}
