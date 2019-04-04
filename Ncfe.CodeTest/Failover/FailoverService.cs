using Ncfe.CodeTest.Configuration;
using Ncfe.CodeTest.Failover.Repository;
using System;
using System.Linq;

namespace Ncfe.CodeTest.Failover
{
    public class FailoverService : IFailoverService
    {
        private readonly IFailoverRepository _failoverRepository;
        private readonly IConfiguration _configuration;

        public FailoverService(IFailoverRepository failoverRepository, IConfiguration configuration)
        {
            _failoverRepository = failoverRepository;
            _configuration = configuration;
        }

        public bool InFailoverMode()
        {
            return _configuration.IsFailoverModeEnabled
                && RecentFailoversCount(_configuration.RecentFailoverPeriod) >= _configuration.FailoverRecordsCountThreshold;
        }

        private int RecentFailoversCount(TimeSpan recentPeriod)
        {
            var failoverEntries = _failoverRepository.GetFailOverEntries();
            DateTime recentCutoff = DateTime.Now - recentPeriod;
            return failoverEntries.Count(fe => fe.DateTime > recentCutoff);
       }
    }
}
