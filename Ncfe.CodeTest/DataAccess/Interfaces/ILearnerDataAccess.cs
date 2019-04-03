namespace Ncfe.CodeTest.DataAccess.Interfaces
{
    public interface ILearnerDataAccess
    {
        LearnerResponse LoadLearner(int learnerId);
    }
}