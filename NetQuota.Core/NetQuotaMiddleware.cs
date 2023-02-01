using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NetQuota.Core.Services;

namespace NetQuota.Core {
    public class NetQuotaMiddleware {

        private readonly RequestDelegate _next;

        public NetQuotaMiddleware(RequestDelegate next) {
            _next = next;
        }

        private async Task SendResourceExhaustedAsync(HttpContext context, string key, Quota quota)
        {
            context.Response.StatusCode = 429;

            await context.Response.WriteAsync($"You have exhausted your quota for '{key}'. Quota resets in {(quota.ExpiresOn - DateTime.UtcNow).TotalSeconds} seconds.");

            return;
        }

        private async Task<Quota> SetQuotaAsync(IQuotaStoreService quotaStoreService, string identifier, string key, Quota quota)
        {
            await quotaStoreService.SetQuotaAsync(identifier, key, quota);

            return quota;
        }

        private async Task<Quota> GetQuotaAsync(IQuotaStoreService quotaStoreService, string identifier, string key)
        {
            if (quotaStoreService != null)
            {
                var contextQuota = await quotaStoreService.GetQuotaAsync(identifier, key);

                return contextQuota;
            }

            return null;
        }

        public async Task Invoke(HttpContext context) {

            var endpoint = context.GetEndpoint();
            var attribute = endpoint?.Metadata.GetMetadata<NetQuotaAttribute>();

            if (attribute != null && !string.IsNullOrEmpty(attribute.Key))
            {
                var amount = attribute.Amount;
                var seconds = attribute.Seconds;
                var key = attribute.Key;

                var identifierService = context.RequestServices.GetService<IQuotaIdentifierService>();

                // Locally, we may not want quotas
                if (identifierService != null)
                {
                    var identifier = await identifierService.GetIdentifierAsync(context);

                    if (!string.IsNullOrEmpty(identifier))
                    {
                        var quotaStoreService = context.RequestServices.GetService<IQuotaStoreService>();

                        var contextQuota = await GetQuotaAsync(quotaStoreService, identifier, key);

                        if(contextQuota != null)
                        {
                            if(contextQuota.AmountLeft <= 0)
                            {
                                // check if has expired
                                if(contextQuota.ExpiresOn <= DateTime.UtcNow)
                                {
                                    contextQuota = new Quota()
                                    {
                                        AmountLeft = amount,
                                        ExpiresOn = DateTime.UtcNow.AddSeconds(seconds)
                                    };
                                } else
                                {
                                    // Quota not expired, send resource exhausted
                                    await SendResourceExhaustedAsync(context, key, contextQuota);

                                    return;
                                }
                            }
                        } else
                        {
                            contextQuota = new Quota()
                            {
                                AmountLeft = amount,
                                ExpiresOn = DateTime.UtcNow.AddSeconds(seconds)
                            };
                        }

                        await _next(context);

                        contextQuota.AmountLeft--;

                        await this.SetQuotaAsync(quotaStoreService, identifier, key, contextQuota);

                        return;
                    }
                }
            } else
            {
                // Not configured, run anyway
                await _next(context);
            }
        }
    }
}