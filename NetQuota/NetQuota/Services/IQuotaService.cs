namespace NetQuota.Services {
    public interface IQuotaService {
        Task<QuotaResult> CheckQuotaAsync(string identifier, string key, int quota, int period);
        Task<string> GetIdentifierAsync(HttpContext context);
    }
}