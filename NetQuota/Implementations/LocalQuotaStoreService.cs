using NetQuota.Services;

namespace NetQuota.Implementations {
    public class LocalQuotaStoreService : IQuotaStoreService
    {
        private Dictionary<string, Dictionary<string, Quota>> _store = new Dictionary<string, Dictionary<string, Quota>>();
        public Task<Quota> GetQuotaAsync(string identifier, string key)
        {
            if (!_store.ContainsKey(identifier))
            {
                return Task.FromResult<Quota>(null);
            }

            if (!_store[identifier].ContainsKey(key))
            {
                return Task.FromResult<Quota>(null);
            }

            return Task.FromResult(_store[identifier][key]);
        }

        public Task SetQuotaAsync(string identifier, string key, Quota quota)
        {
            if (!_store.ContainsKey(identifier))
            {
                _store[identifier] = new Dictionary<string, Quota>();
            }

            _store[identifier][key] = quota;
            
            return Task.CompletedTask;
        }
    }
}