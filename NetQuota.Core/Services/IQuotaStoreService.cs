namespace NetQuota.Core.Services {
    public interface IQuotaStoreService {
        Task<QuotaInstance> GetQuotaAsync(string identifier, string key);
        Task SetQuotaAsync(string identifier, string key, QuotaInstance quota);
    }
}