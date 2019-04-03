using System.Collections.Generic;

namespace Ncfe.CodeTest.Failover.Repository
{
    public interface IFailoverRepository
    {
        List<FailoverEntry> GetFailOverEntries();
    }
}
