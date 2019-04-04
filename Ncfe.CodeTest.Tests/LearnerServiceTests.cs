﻿using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ncfe.CodeTest.DataAccess.Interfaces;
using Ncfe.CodeTest.Failover;
using Ncfe.CodeTest.Model;
using System;
using System.Collections.Generic;

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

        private ILearnerArchiveService _archiveService;
        private ILearnerDataService _failoverDataService;
        private ILearnerDataService _liveDataService;
        private IFailoverService _failoverService;

        private LearnerService _sut;

        [TestInitialize]
        public void Setup()
        {
            _archiveService = A.Fake<ILearnerArchiveService>();
            _failoverDataService = A.Fake<ILearnerDataService>();
            _liveDataService = A.Fake<ILearnerDataService>();
            _failoverService = A.Fake<IFailoverService>();

            _sut = new LearnerService(_archiveService, _failoverDataService, _liveDataService, _failoverService);
        }

        [TestCleanup]
        public void Teardown()
        {
            _archiveService = null;
            _failoverDataService = null;
            _liveDataService = null;
            _failoverService = null;

            _sut = null;
        }

        public static IEnumerable<object[]> GetLearnerAllDecisions()
        {
            bool[] all = new bool[] { false, true };

            foreach (var isLearnerArchived in all)
            {
                foreach (var inFailoverMode in all)
                {
                    foreach (var learnerResponseArchived in all)
                    {
                        yield return new object[]
                        {
                                isLearnerArchived
                            , inFailoverMode
                            , learnerResponseArchived
                        };
                    }
                }
            }
        }

        [DataTestMethod, DynamicData(nameof(GetLearnerAllDecisions), DynamicDataSourceType.Method)]
        public void GetLearnerTests(
              bool isLearnerArchived
            , bool inFailoverMode
            , bool learnerResponseArchived
            )
        {
            var archiveLearner = new Learner();
            A.CallTo(() => _archiveService.GetLearner(A<int>._)).Returns(archiveLearner);

            var failoverLearner = new Learner();
            var failoverLearnerResponse = new LearnerResponse { IsArchived = learnerResponseArchived, Learner = failoverLearner };
            A.CallTo(() => _failoverDataService.GetLearner(A<int>._)).Returns(failoverLearnerResponse);

            var liveLearner = new Learner();
            var liveLearnerResponse = new LearnerResponse { IsArchived = learnerResponseArchived, Learner = liveLearner };
            A.CallTo(() => _liveDataService.GetLearner(A<int>._)).Returns(liveLearnerResponse);

            A.CallTo(() => _failoverService.InFailoverMode()).Returns(inFailoverMode);

            LearnerSource learnerSource = isLearnerArchived || learnerResponseArchived ? LearnerSource.Archive :
                                            inFailoverMode ? LearnerSource.Failover :
                                            LearnerSource.Live;

            bool archiveDataAccessed = isLearnerArchived || learnerResponseArchived;
            bool failoverDataAccessed = !isLearnerArchived && inFailoverMode;
            bool liveDataAccessed = !isLearnerArchived && !inFailoverMode;

            bool failoverServiceAccessed = !isLearnerArchived;

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
                A.CallTo(() => _archiveService.GetLearner(A<int>._)).MustHaveHappenedOnceExactly();
            else
                A.CallTo(() => _archiveService.GetLearner(A<int>._)).MustNotHaveHappened();

            if (failoverDataAccessed)
                A.CallTo(() => _failoverDataService.GetLearner(A<int>._)).MustHaveHappenedOnceExactly();
            else
                A.CallTo(() => _failoverDataService.GetLearner(A<int>._)).MustNotHaveHappened();

            if (liveDataAccessed)
                A.CallTo(() => _liveDataService.GetLearner(A<int>._)).MustHaveHappenedOnceExactly();
            else
                A.CallTo(() => _liveDataService.GetLearner(A<int>._)).MustNotHaveHappened();

            if (failoverServiceAccessed)
                A.CallTo(() => _failoverService.InFailoverMode()).MustHaveHappenedOnceExactly();
            else
                A.CallTo(() => _failoverService.InFailoverMode()).MustNotHaveHappened();
        }
    }
}
