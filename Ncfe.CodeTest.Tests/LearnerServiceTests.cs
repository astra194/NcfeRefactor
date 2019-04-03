using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ncfe.CodeTest.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Ncfe.CodeTest.Tests
{
    [TestClass]
    public class LearnerServiceTests
    {
        public enum LearnerSource
        {
            Archive,
            Live,
            Failover,
        }

        private ILearnerDataService _archiveDataService;
        private ILearnerDataService _failoverDataService;
        private ILearnerDataService _liveDataService;
        private IFailoverRepository _failoverRepository;

        private LearnerService _sut;

        [TestInitialize]
        public void Setup()
        {
            _archiveDataService = A.Fake<ILearnerDataService>();
            _failoverDataService = A.Fake<ILearnerDataService>();
            _liveDataService = A.Fake<ILearnerDataService>();
            _failoverRepository = A.Fake<IFailoverRepository>();

            _sut = new LearnerService(_archiveDataService, _failoverDataService, _liveDataService, _failoverRepository);
        }

        [TestCleanup]
        public void Teardown()
        {
            _archiveDataService = null;
            _failoverDataService = null;
            _liveDataService = null;
            _failoverRepository = null;

            _sut = null;
        }

        public static IEnumerable<object[]> GetLearnerAllDecisions()
        {
            bool[] all = new bool[] { false, true };

            foreach (var isLearnerArchived in all)
            {
                foreach (var failoverEntriesRecent in all)
                {
                    foreach (var failoverEntriesThreshold in all)
                    {
                        foreach (var failoverModeEnabled in all)
                        {
                            foreach (var learnerResponseArchived in all)
                            {
                                yield return new object[]
                                {
                                      isLearnerArchived
                                    , failoverEntriesRecent
                                    , failoverEntriesThreshold
                                    , failoverModeEnabled
                                    , learnerResponseArchived
                                };
                            }
                        }
                    }
                }
            }
        }

        [DataTestMethod, DynamicData(nameof(GetLearnerAllDecisions), DynamicDataSourceType.Method)]
        public void GetLearnerTests(
              bool isLearnerArchived
            , bool failoverEntriesRecent
            , bool failoverEntriesThreshold
            , bool failoverModeEnabled
            , bool learnerResponseArchived
            )
        {
            var archiveLearner = new Learner();
            var archiveLearnerResponse = new LearnerResponse { IsArchived = true, Learner = archiveLearner };
            A.CallTo(() => _archiveDataService.GetLearner(A<int>._)).Returns(archiveLearnerResponse);

            var failoverLearner = new Learner();
            var failoverLearnerResponse = new LearnerResponse { IsArchived = learnerResponseArchived, Learner = failoverLearner };
            A.CallTo(() => _failoverDataService.GetLearner(A<int>._)).Returns(failoverLearnerResponse);

            var liveLearner = new Learner();
            var liveLearnerResponse = new LearnerResponse { IsArchived = learnerResponseArchived, Learner = liveLearner };
            A.CallTo(() => _liveDataService.GetLearner(A<int>._)).Returns(liveLearnerResponse);

            var failoverEntries = new List<FailoverEntry>(GenerateFailoverEntries(failoverEntriesThreshold, failoverEntriesRecent));
            A.CallTo(() => _failoverRepository.GetFailOverEntries()).Returns(failoverEntries);

            ConfigurationManager.AppSettings["IsFailoverModeEnabled"] = failoverModeEnabled.ToString();

            LearnerSource learnerSource = isLearnerArchived || learnerResponseArchived ? LearnerSource.Archive :
                                            failoverEntriesRecent && failoverEntriesThreshold && failoverModeEnabled ? LearnerSource.Failover :
                                            LearnerSource.Live;

            bool archiveDataAccessed = isLearnerArchived || learnerResponseArchived;
            bool failoverDataAccessed = !isLearnerArchived && failoverEntriesRecent && failoverEntriesThreshold && failoverModeEnabled;
            bool liveDataAccessed = !isLearnerArchived && (!failoverEntriesRecent || !failoverEntriesThreshold || !failoverModeEnabled);

            bool failoverRepositoryAccessed = !isLearnerArchived;

            var learner = _sut.GetLearner(1, isLearnerArchived);

            Learner expectedLearner;
            switch (learnerSource)
            {
                case LearnerSource.Archive:
                    expectedLearner = archiveLearner;
                    break;

                case LearnerSource.Failover:
                    expectedLearner = failoverLearner;
                    break;

                case LearnerSource.Live:
                    expectedLearner = liveLearner;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(learnerSource));
            }
            Assert.AreSame(expectedLearner, learner);

            if (archiveDataAccessed)
                A.CallTo(() => _archiveDataService.GetLearner(A<int>._)).MustHaveHappenedOnceExactly();
            else
                A.CallTo(() => _archiveDataService.GetLearner(A<int>._)).MustNotHaveHappened();

            if (failoverDataAccessed)
                A.CallTo(() => _failoverDataService.GetLearner(A<int>._)).MustHaveHappenedOnceExactly();
            else
                A.CallTo(() => _failoverDataService.GetLearner(A<int>._)).MustNotHaveHappened();

            if (liveDataAccessed)
                A.CallTo(() => _liveDataService.GetLearner(A<int>._)).MustHaveHappenedOnceExactly();
            else
                A.CallTo(() => _liveDataService.GetLearner(A<int>._)).MustNotHaveHappened();

            if (failoverRepositoryAccessed)
                A.CallTo(() => _failoverRepository.GetFailOverEntries()).MustHaveHappenedOnceExactly();
            else
                A.CallTo(() => _failoverRepository.GetFailOverEntries()).MustNotHaveHappened();
        }

        private IEnumerable<FailoverEntry> GenerateFailoverEntries(bool failoverEntriesThreshold, bool failoverEntriesRecent)
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
