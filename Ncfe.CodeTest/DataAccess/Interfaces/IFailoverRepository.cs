using System.Collections.Generic;

namespace Ncfe.CodeTest.DataAccess.Interfaces
{
    public interface IFailoverRepository
    {
        List<FailoverEntry> GetFailOverEntries();
    }
}
