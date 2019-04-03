using Ncfe.CodeTest.DataAccess.Interfaces;
using System.Collections.Generic;

namespace Ncfe.CodeTest.DataAccess.Actuals
{
    public class FailoverRepository : IFailoverRepository
    {
        public List<FailoverEntry> GetFailOverEntries()
        {
            // return all from fail entries from database
            return new List<FailoverEntry>();
        }
    }
}
