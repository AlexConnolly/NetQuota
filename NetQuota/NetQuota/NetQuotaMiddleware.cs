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

                var quotaService = context.RequestServices.GetRequiredService<IQuotaService>();
                
                // Locally, we may not want quotas
                if(quotaService != null) {
                    var identifier = await quotaService.GetIdentifierAsync(context);

                    if(!string.IsNullOrEmpty(identifier)) {
                        var quotaResult = await quotaService.CheckQuotaAsync(identifier, key, quota, period);

                        if(!quotaResult.IsAllowed) {
                            context.Response.StatusCode = 429;
                            await context.Response.WriteAsync($"You have exhausted your quota for '{key}'. Quota resets in {quotaResult.ResetSeconds} seconds.");
                            return;
                        }
                    }
                }
            }

            await _next(context);
        }
    }
}