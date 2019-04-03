using Ncfe.CodeTest.DataAccess.Actuals;
using Ncfe.CodeTest.DataAccess.Interfaces;

namespace Ncfe.CodeTest.DataAccess.Proxies
{
    public class FailoverDataService : ILearnerDataService
    {
        public LearnerResponse GetLearner(int learnerId)
        {
            return FailoverLearnerDataAccess.GetLearnerById(learnerId);
        }
    }
}
