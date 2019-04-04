using Ncfe.CodeTest.Model;

namespace Ncfe.CodeTest.DataAccess.Interfaces
{
    public interface ILearnerDataService
    {
        LearnerResponse GetLearner(int learnerId);
    }
}
