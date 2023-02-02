

using NetQuota.Core.Services;
using NetQuota.Core;

namespace NetQuota.Implementations {
    public class LocalQuotaStoreService : IQuotaStoreService
    {
        private Dictionary<string, Dictionary<string, QuotaInstance>> _store = new Dictionary<string, Dictionary<string, QuotaInstance>>();
        public async Task<QuotaInstance> GetQuotaAsync(string identifier, string key)
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

        public async Task SetQuotaAsync(string identifier, string key, QuotaInstance quota)
        {
            if (!_store.ContainsKey(identifier))
            {
                _store[identifier] = new Dictionary<string, QuotaInstance>();
            }

            _store[identifier][key] = quota;
        }
    }
}