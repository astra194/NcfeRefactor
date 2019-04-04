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
            Func<int, Learner> strategy;
            if (isLearnerArchived)
            {
                strategy = (id) => _archiveService.GetLearner(id);
            }
            else if (_failoverService.InFailoverMode())
            {
                strategy = (id) => DataOrArchive(_failoverDataService, id);
            }
            else
            {
                strategy = (id) => DataOrArchive(_liveDataService, id);
            }

            return strategy(learnerId);
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
