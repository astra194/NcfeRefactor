using Ncfe.CodeTest.DataAccess.Interfaces;
using Ncfe.CodeTest.Failover;
using System;
using System.Configuration;

namespace Ncfe.CodeTest
{
    public class LearnerService
    {
        private readonly ILearnerDataService _archiveDataService;
        private readonly ILearnerDataService _failoverDataService;
        private readonly ILearnerDataService _liveDataService;
        private readonly IFailoverService _failoverService;

        public LearnerService(
             ILearnerDataService archiveDataService
           , ILearnerDataService failoverDataService
           , ILearnerDataService liveDataService
           , IFailoverService failoverService
            )
        {
            _archiveDataService = archiveDataService;
            _failoverDataService = failoverDataService;
            _liveDataService = liveDataService;
            _failoverService = failoverService;
        }

        public Learner GetLearner(int learnerId, bool isLearnerArchived)
        {
            if (isLearnerArchived)
            {
                LearnerResponse archiveLearnerResponse = _archiveDataService.GetLearner(learnerId);
                return archiveLearnerResponse.Learner;
            }
            else
            {
                LearnerResponse learnerResponse = null;
                Learner learner = null;

                if (_failoverService.InFailoverMode() && (ConfigurationManager.AppSettings["IsFailoverModeEnabled"] == "true" || ConfigurationManager.AppSettings["IsFailoverModeEnabled"] == "True"))
                {
                    learnerResponse = _failoverDataService.GetLearner(learnerId);
                }
                else
                {
                    learnerResponse = _liveDataService.GetLearner(learnerId);
                }

                if (learnerResponse.IsArchived)
                {
                    learner = _archiveDataService.GetLearner(learnerId).Learner;
                }
                else
                {
                    learner = learnerResponse.Learner;
                }

                return learner;
            }
        }
    }
}
