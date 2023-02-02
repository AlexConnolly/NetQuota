using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NetQuota.Core.Services;

namespace NetQuota.Core {
    public class NetQuotaMiddleware {

        private readonly RequestDelegate _next;

        public NetQuotaMiddleware(RequestDelegate next) {
            _next = next;
        }

        private async Task SendResourceExhaustedAsync(HttpContext context, string key, QuotaInstance quota)
        {
            context.Response.StatusCode = 429;

            await context.Response.WriteAsync($"You have exhausted your quota for '{key}'. Quota resets in {(quota.ExpiresOn - DateTime.UtcNow).TotalSeconds} seconds.");

            return;
        }

        private async Task<QuotaInstance> SetQuotaAsync(IQuotaStoreService quotaStoreService, string identifier, string key, QuotaInstance quota)
        {
            await quotaStoreService.SetQuotaAsync(identifier, key, quota);

            return quota;
        }

        private async Task<QuotaInstance> GetQuotaAsync(IQuotaStoreService quotaStoreService, string identifier, string key)
        {
            if (quotaStoreService != null)
            {
                var contextQuota = await quotaStoreService.GetQuotaAsync(identifier, key);

                return contextQuota;
            }

            return null;
        }

        private QuotaDefinition GetDefinitionFromProfile(QuotaProfile profile, string resourceKey)
        {
            if (profile == null)
                return null;

            if (profile.Quotas == null)
                return null;

            foreach(var definition in profile.Quotas)
            {
                if (definition.ResourceKey.ToLower() == resourceKey.ToLower())
                    return definition;
            }

            return null;
        }

        private QuotaDefinition GetQuotaDefinitionFromIdentifier(IQuotaProfileService profileService, string identifier, string resourceKey)
        {
            var defaultQuota = profileService.GetDefaultProfile();
            var identifierQuota = profileService.GetProfileForIdentifier(identifier);

            var defaultDefinition = this.GetDefinitionFromProfile(defaultQuota, resourceKey);
            var identifierDefinition = this.GetDefinitionFromProfile(identifierQuota, resourceKey);

            // Here, we prioritise the profile based definition
            if (identifierDefinition != null)
                return identifierDefinition;

            // If we have no specific profile-based definition we can use the default definition
            return defaultDefinition;
        }

        public async Task Invoke(HttpContext context) {

            var endpoint = context.GetEndpoint();
            var attribute = endpoint?.Metadata.GetMetadata<NetQuotaAttribute>();

            if (attribute != null && !string.IsNullOrEmpty(attribute.ResourceKey))
            {
                var identifierService = context.RequestServices.GetService<IQuotaIdentifierService>();
                var profileService = context.RequestServices.GetService<IQuotaProfileService>();

                // Locally, we may not want quotas
                if (identifierService != null)
                {
                    var identifier = await identifierService.GetIdentifierAsync(context);

                    if (!string.IsNullOrEmpty(identifier))
                    {
                        var quotaStoreService = context.RequestServices.GetService<IQuotaStoreService>();

                        var contextQuota = await GetQuotaAsync(quotaStoreService, identifier, attribute.ResourceKey);

                        if(contextQuota != null)
                        {
                            if(contextQuota.AmountLeft <= 0)
                            {
                                // check if has expired
                                if(contextQuota.ExpiresOn <= DateTime.UtcNow)
                                {
                                    var quotaDefinition = this.GetQuotaDefinitionFromIdentifier(profileService, identifier, attribute.ResourceKey);

                                    contextQuota = new QuotaInstance()
                                    {
                                        AmountLeft = quotaDefinition.Amount,
                                        ExpiresOn = DateTime.UtcNow.AddSeconds(quotaDefinition.Seconds)
                                    };
                                } else
                                {
                                    // Quota not expired, send resource exhausted
                                    await SendResourceExhaustedAsync(context, attribute.ResourceKey, contextQuota);

                                    return;
                                }
                            }
                        } else
                        {
                            var quotaDefinition = this.GetQuotaDefinitionFromIdentifier(profileService, identifier, attribute.ResourceKey);

                            contextQuota = new QuotaInstance()
                            {
                                AmountLeft = quotaDefinition.Amount,
                                ExpiresOn = DateTime.UtcNow.AddSeconds(quotaDefinition.Seconds)
                            };
                        }

                        await _next(context);

                        contextQuota.AmountLeft--;

                        await this.SetQuotaAsync(quotaStoreService, identifier, attribute.ResourceKey, contextQuota);

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