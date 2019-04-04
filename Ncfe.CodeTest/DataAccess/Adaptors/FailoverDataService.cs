using Ncfe.CodeTest.DataAccess.Actuals;
using Ncfe.CodeTest.DataAccess.Interfaces;
using Ncfe.CodeTest.Model;

namespace Ncfe.CodeTest.DataAccess.Adaptors
{
    public class FailoverDataService : ILearnerDataService
    {
        public LearnerResponse GetLearner(int learnerId)
        {
            return FailoverLearnerDataAccess.GetLearnerById(learnerId);
        }
    }
}
