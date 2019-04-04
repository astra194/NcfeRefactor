using Ncfe.CodeTest.DataAccess.Actuals;
using Ncfe.CodeTest.DataAccess.Interfaces;
using Ncfe.CodeTest.Model;

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
