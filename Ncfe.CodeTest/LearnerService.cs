using Ncfe.CodeTest.DataAccess.Interfaces;
using System;
using System.Configuration;

namespace Ncfe.CodeTest
{
    public class LearnerService
    {
        private readonly ILearnerDataService _archiveDataService;
        private readonly ILearnerDataService _failoverDataService;
        private readonly ILearnerDataService _liveDataService;
        private readonly IFailoverRepository _failoverRepository;

        public LearnerService(
             ILearnerDataService archiveDataService
           , ILearnerDataService failoverDataService
           , ILearnerDataService liveDataService
           , IFailoverRepository failoverRepository
            )
        {
            _archiveDataService = archiveDataService;
            _failoverDataService = failoverDataService;
            _liveDataService = liveDataService;
            _failoverRepository = failoverRepository;
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
                var failoverEntries = _failoverRepository.GetFailOverEntries();

                var failedRequests = 0;

                foreach (var failoverEntry in failoverEntries)
                {
                    if (failoverEntry.DateTime > DateTime.Now.AddMinutes(-10))
                    {
                        failedRequests++;
                    }
                }

                LearnerResponse learnerResponse = null;
                Learner learner = null;

                if (failedRequests > 100 && (ConfigurationManager.AppSettings["IsFailoverModeEnabled"] == "true" || ConfigurationManager.AppSettings["IsFailoverModeEnabled"] == "True"))
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
