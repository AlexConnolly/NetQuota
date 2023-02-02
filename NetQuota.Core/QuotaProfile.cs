using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetQuota.Core
{
    /// <summary>
    /// A collection of quota definitions assigned to a specific profile type (eg. enterprise users may get higher quotas)
    /// </summary>
    public class QuotaProfile
    {
        public string ProfileName { get; private set; }

        public IEnumerable<QuotaDefinition> Quotas { get; private set; }

        public QuotaProfile(string profileName, IEnumerable<QuotaDefinition> quotas)
        {
            this.ProfileName = profileName;
            this.Quotas = quotas;
        }
    }
}
