using NetQuota.Services;

namespace NetQuota {
    public class NetQuotaMiddleware {

        private readonly RequestDelegate _next;

        public NetQuotaMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task Invoke(HttpContext context) { 

            var endpoint = context.GetEndpoint();
            var attribute = endpoint?.Metadata.GetMetadata<NetQuotaAttribute>();

            if(attribute != null && !string.IsNullOrEmpty(attribute.Key)) {

                var quota = attribute.Quota;
                var period = attribute.Period;
                var key = attribute.Key;

                var identifierService = context.RequestServices.GetService<IQuotaIdentifierService>();
                
                // Locally, we may not want quotas
                if(identifierService != null) {
                    var identifier = await identifierService.GetIdentifierAsync(context);

                    if(!string.IsNullOrEmpty(identifier)) {

                        var quotaStoreService = context.RequestServices.GetService<IQuotaStoreService>();

                        var quotaResult = await quotaStoreService.GetQuotaAsync(identifier, key);

                        if(quotaResult != null) {
                            
                            // Check if we have quota
                            if(quotaResult.AmountLeft != 0) {
                                await _next(context);
                                await quotaStoreService.SetQuotaAsync(identifier, key, new Quota() { ExpiresOn = quotaResult.ExpiresOn, AmountLeft = quotaResult.AmountLeft - 1 });
                            } else {

                                bool hasQuotaExpired = DateTime.UtcNow > quotaResult.ExpiresOn;

                                if(hasQuotaExpired) {
                                    await _next(context);
                                    await quotaStoreService.SetQuotaAsync(identifier, key, new Quota() { ExpiresOn = DateTime.UtcNow.AddSeconds(period), AmountLeft = quota -1 });
                                } else {
                                    context.Response.StatusCode = 429;
                                    await context.Response.WriteAsync($"You have exhausted your quota for '{key}'. Quota resets in {(quotaResult.ExpiresOn - DateTime.UtcNow).TotalSeconds} seconds.");
                                    return;
                                }
                            }
                        } else {
                            await _next(context);
                            await quotaStoreService.SetQuotaAsync(identifier, key, new Quota() { ExpiresOn = DateTime.UtcNow.AddSeconds(period), AmountLeft = quota - 1 });
                        }
                    }
                }
            }

            await _next(context);
        }
    }
}