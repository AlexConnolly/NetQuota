namespace NetQuota.Services {
    public interface IQuotaIdentifierService {
        Task<string> GetIdentifierAsync(HttpContext context);
    }
}