using Ncfe.CodeTest.Failover.Repository;
using System;
using System.Linq;

namespace Ncfe.CodeTest.Failover
{
    public class FailoverService : IFailoverService
    {
        private readonly IFailoverRepository _failoverRepository;

        public FailoverService(IFailoverRepository failoverRepository)
        {
            _failoverRepository = failoverRepository;
        }

        public bool InFailoverMode()
        {
            var failoverEntries = _failoverRepository.GetFailOverEntries();
            DateTime recentCutoff = DateTime.Now - TimeSpan.FromMinutes(10);
            var failedRequests = failoverEntries.Count(fe => fe.DateTime > recentCutoff);
            return failedRequests > 100;
        }
    }
}
