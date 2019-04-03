using Ncfe.CodeTest.DataAccess.Actuals;
using Ncfe.CodeTest.DataAccess.Interfaces;

namespace Ncfe.CodeTest.DataAccess.Wrappers
{
    public class LiveDataService : ILearnerDataService
    {
        public LearnerResponse GetLearner(int learnerId)
        {
            var actual = new LearnerDataAccess();
            return actual.LoadLearner(learnerId);
        }
    }
}
