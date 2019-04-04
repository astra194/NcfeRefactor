using Ncfe.CodeTest.Model;

namespace Ncfe.CodeTest.DataAccess.Interfaces
{
    public interface ILearnerArchiveService
    {
        Learner GetLearner(int learnerId);
    }
}
