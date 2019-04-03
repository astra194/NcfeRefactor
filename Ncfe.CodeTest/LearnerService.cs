using Ncfe.CodeTest.DataAccess.Interfaces;
using System;
using System.Configuration;

namespace Ncfe.CodeTest
{
    public class LearnerService
    {
        private readonly IArchivedDataService _archivedDataService;
        private readonly IFailoverLearnerDataAccess _failoverLearnerDataAccess;
        private readonly ILearnerDataAccess _learnerDataAccess;

        public LearnerService(IArchivedDataService archivedDataService, IFailoverLearnerDataAccess failoverLearnerDataAccess, ILearnerDataAccess learnerDataAccess)
        {
            _archivedDataService = archivedDataService;
            _failoverLearnerDataAccess = failoverLearnerDataAccess;
            _learnerDataAccess = learnerDataAccess;
        }

        public Learner GetLearner(int learnerId, bool isLearnerArchived)
        {
            Learner archivedLearner = null;

            if (isLearnerArchived)
            {
                archivedLearner = _archivedDataService.GetArchivedLearner(learnerId);

                return archivedLearner;
            }
            else
            {

                var failoverRespository = new FailoverRepository();
                var failoverEntries = failoverRespository.GetFailOverEntries();


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
                    learnerResponse = _failoverLearnerDataAccess.GetLearnerById(learnerId);
                }
                else
                {
                    learnerResponse = _learnerDataAccess.LoadLearner(learnerId);


                }

                if (learnerResponse.IsArchived)
                {
                    learner = _archivedDataService.GetArchivedLearner(learnerId);
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
