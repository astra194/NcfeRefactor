using Ncfe.CodeTest.DataAccess.Actuals;
using Ncfe.CodeTest.DataAccess.Interfaces;

namespace Ncfe.CodeTest.DataAccess.Adaptors
{
    public class ArchiveDataService : ILearnerArchiveService
    {
        public Learner GetLearner(int learnerId)
        {
            var archiveService = new ArchivedDataService();
            return archiveService.GetArchivedLearner(learnerId);
        }
    }
}
