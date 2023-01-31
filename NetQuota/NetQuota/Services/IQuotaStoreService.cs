namespace NetQuota.Services {
    public interface IQuotaStoreService {
        Task<Quota> GetQuotaAsync(string identifier, string key);
        Task SetQuotaAsync(string identifier, string key, Quota quota);
    }
}