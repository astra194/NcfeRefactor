using Ncfe.CodeTest.DataAccess.Actuals;
using Ncfe.CodeTest.DataAccess.Interfaces;

namespace Ncfe.CodeTest.DataAccess.Proxies
{
    public class ArchiveDataService : ILearnerDataService
    {
        public LearnerResponse GetLearner(int learnerId)
        {
            var archiveService = new ArchivedDataService();
            var learner = archiveService.GetArchivedLearner(learnerId);
            return new LearnerResponse { IsArchived = true, Learner = learner };
        }
    }
}
