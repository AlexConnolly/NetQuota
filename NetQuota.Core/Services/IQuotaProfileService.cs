using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetQuota.Core.Services
{
    /// <summary>
    /// Used to register profiles 
    /// </summary>
    public interface IQuotaProfileService
    {
        /// <summary>
        /// Gets the default profile that applies to all users
        /// </summary>
        /// <returns></returns>
        QuotaProfile GetDefaultProfile();

        /// <summary>
        /// Gets a profile for a specific identifier (note: identifier is not hte name of the profile)
        /// </summary>
        /// <param name="identifier">The identifier from the request</param>
        /// <returns></returns>
        QuotaProfile GetProfileForIdentifier(string identifier);
    }
}
