

using NetQuota.Core.Services;
using NetQuota.Core;

namespace NetQuota.Implementations {
    public class LocalQuotaStoreService : IQuotaStoreService
    {
        private Dictionary<string, Dictionary<string, Quota>> _store = new Dictionary<string, Dictionary<string, Quota>>();
        public async Task<Quota> GetQuotaAsync(string identifier, string key)
        {
            if (!_store.ContainsKey(identifier))
            {
                return null;
            }

            if (!_store[identifier].ContainsKey(key))
            {
                return null;
            }

            return _store[identifier][key];
        }

        public async Task SetQuotaAsync(string identifier, string key, Quota quota)
        {
            if (!_store.ContainsKey(identifier))
            {
                _store[identifier] = new Dictionary<string, Quota>();
            }

            _store[identifier][key] = quota;
        }
    }
}