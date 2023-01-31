namespace NetQuota.Core.Services {
    public interface IQuotaIdentifierService {
        Task<string> GetIdentifierAsync(Microsoft.AspNetCore.Http.HttpContext context);
    }
}