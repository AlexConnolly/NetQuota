using NetQuota.Core.Services;

namespace NetQuota.Implementations {
    public class QuotaIdentifierService : IQuotaIdentifierService
    {
        public async Task<string> GetIdentifierAsync(HttpContext context)
        {
            return context.Connection.RemoteIpAddress.ToString();
        }
    }
}