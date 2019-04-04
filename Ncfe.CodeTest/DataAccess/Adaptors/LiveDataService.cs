using Ncfe.CodeTest.DataAccess.Actuals;
using Ncfe.CodeTest.DataAccess.Interfaces;
using Ncfe.CodeTest.Model;

namespace Ncfe.CodeTest.DataAccess.Adaptors
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
