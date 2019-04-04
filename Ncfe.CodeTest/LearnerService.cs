using Ncfe.CodeTest.DataAccess.Interfaces;
using Ncfe.CodeTest.Failover;

namespace Ncfe.CodeTest
{
    public class LearnerService
    {
        private readonly ILearnerArchiveService _archiveService;
        private readonly ILearnerDataService _failoverDataService;
        private readonly ILearnerDataService _liveDataService;
        private readonly IFailoverService _failoverService;

        public LearnerService(
             ILearnerArchiveService archiveService
           , ILearnerDataService failoverDataService
           , ILearnerDataService liveDataService
           , IFailoverService failoverService
            )
        {
            _archiveService = archiveService;
            _failoverDataService = failoverDataService;
            _liveDataService = liveDataService;
            _failoverService = failoverService;
        }

        public Learner GetLearner(int learnerId, bool isLearnerArchived)
        {
            if (isLearnerArchived)
            {
                return _archiveService.GetLearner(learnerId);
            }
            else
            {
                LearnerResponse learnerResponse = null;
                Learner learner = null;

                if (_failoverService.InFailoverMode())
                {
                    learnerResponse = _failoverDataService.GetLearner(learnerId);
                }
                else
                {
                    learnerResponse = _liveDataService.GetLearner(learnerId);
                }

                if (learnerResponse.IsArchived)
                {
                    learner = _archiveService.GetLearner(learnerId);
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
