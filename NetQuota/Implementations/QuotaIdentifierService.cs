using NetQuota.Core.Services;

namespace NetQuota.Implementations {
    public class QuotaIdentifierService : IQuotaIdentifierService
    {
        public Task<string> GetIdentifierAsync(HttpContext context)
        {
            return Task.FromResult(context.Connection.RemoteIpAddress.ToString());
        }
    }
}