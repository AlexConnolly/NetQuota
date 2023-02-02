using NetQuota.Core;
using NetQuota.Core.Services;

namespace NetQuota.Implementations
{
    public class QuotaProfileService : IQuotaProfileService
    {
        public QuotaProfile GetDefaultProfile()
        {
            return new QuotaProfile("Default", new List<QuotaDefinition>()
            {
                new QuotaDefinition("Weather", 60, 5)
            });
        }

        public QuotaProfile GetProfileForIdentifier(string identifier)
        {
            switch(identifier)
            {
                case "Enterprise":
                    return new QuotaProfile("Enterprise", new List<QuotaDefinition>()
                    {
                        new QuotaDefinition("Weather", 60, 10)
                    });

                default:
                    // We can return null as we have a default profile 
                    return null;
                    
            }
        }
    }
}
