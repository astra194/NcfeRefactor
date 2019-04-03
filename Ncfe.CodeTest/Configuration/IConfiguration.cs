using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ncfe.CodeTest.Configuration
{
    public interface IConfiguration
    {
        /// <summary>
        /// Up to how long ago can a failover record have been written to be considered recent.
        /// </summary>
        TimeSpan RecentFailoverPeriod { get; }

        /// <summary>
        /// How many recent failover records must there be to enter failover mode
        /// </summary>
        int FailoverRecordsCountThreshold { get; }

        /// <summary>
        /// Will failover mode be used if all other criteria are met
        /// </summary>
        bool IsFailoverModeEnabled { get; }
    }
}
