using Ncfe.CodeTest.DataAccess.Actuals;
using Ncfe.CodeTest.DataAccess.Interfaces;

namespace Ncfe.CodeTest.DataAccess.Proxies
{
    public class FailoverLearnerDataAccessWrapper : IFailoverLearnerDataAccess
    {
        public LearnerResponse GetLearnerById(int id)
        {
            return FailoverLearnerDataAccess.GetLearnerById(id);
        }
    }
}
