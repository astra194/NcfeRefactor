using Ncfe.CodeTest.DataAccess.Actuals;
using Ncfe.CodeTest.DataAccess.Interfaces;

namespace Ncfe.CodeTest.DataAccess.Proxies
{
    public class ArchivedDataServiceWrapper : IArchivedDataService
    {
        public Learner GetArchivedLearner(int learnerId)
        {
            var actual = new ArchivedDataService();
            return actual.GetArchivedLearner(learnerId);
        }
    }
}
