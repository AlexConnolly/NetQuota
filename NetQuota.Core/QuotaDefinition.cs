using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetQuota.Core
{
    /// <summary>
    /// Defines a quota, its resource,
    /// </summary>
    public class QuotaDefinition
    {
        /// <summary>
        /// The key of the resource (eg. 'Orders')
        /// </summary>
        public string ResourceKey { get; private set; }

        /// <summary>
        /// The period of time for which the quota exists
        /// </summary>
        public int Seconds { get; private set; }

        /// <summary>
        /// The amount the resource can be consumed in given seconds
        /// </summary>
        public int Amount { get; private set; }

        public QuotaDefinition(string resourceKey, int seconds, int amount)
        {
            this.ResourceKey = resourceKey;
            this.Seconds = seconds;
            this.Amount = amount;
        }
    }
}
