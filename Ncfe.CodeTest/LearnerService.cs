using Ncfe.CodeTest.DataAccess.Interfaces;
using Ncfe.CodeTest.Failover;
using Ncfe.CodeTest.Model;
using System;

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
            Func<int, Learner> strategy = GetStrategy(isLearnerArchived, () => _failoverService.InFailoverMode());
            return strategy(learnerId);
        }

        // inFailoverMode passed as a Func as we only want to call it (expensive) when required
        private Func<int, Learner> GetStrategy(bool isLearnerArchived, Func<bool> inFailoverMode)
        {
            if (isLearnerArchived)
            {
                return (id) => _archiveService.GetLearner(id);
            }
            else
            {
                ILearnerDataService dataService = inFailoverMode()
                    ? _failoverDataService
                    : _liveDataService;
                return (id) => DataOrArchive(dataService, id);
            }
        }

        private Learner DataOrArchive(ILearnerDataService dataService, int learnerId)
        {
            LearnerResponse response = dataService.GetLearner(learnerId);
            return response.IsArchived
                ? _archiveService.GetLearner(learnerId)
                : response.Learner;
        }
    }
}
