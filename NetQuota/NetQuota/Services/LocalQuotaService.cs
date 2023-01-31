namespace NetQuota.Services {
    public class LocalQuotaService : IQuotaService
    {
        private class QuotaTracker {
            public int Quota { get; set; }
            public int EndTime { get; set; }
        }

        private Dictionary<string, Dictionary<string, QuotaTracker>> _quota = new Dictionary<string, Dictionary<string, QuotaTracker>>();

        public Task<QuotaResult> CheckQuotaAsync(string identifier, string key, int quota, int period)
        {
            //  If the identifier doesn't exist, add it
            if(!_quota.ContainsKey(identifier))
            {
                _quota.Add(identifier, new Dictionary<string, QuotaTracker>());
            }

            // If the key doesn't exist, add it
            if(!_quota[identifier].ContainsKey(key))
            {
                _quota[identifier].Add(key, new QuotaTracker() { Quota = quota, EndTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds + period });
            }

            // Get the quota tracker
            var quotaTracker = _quota[identifier][key];

            // If the quota has expired, reset it
            if(quotaTracker.EndTime < (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds)
            {
                quotaTracker.Quota = quota;
                quotaTracker.EndTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds + period;
            }

            // If the quota is 0, return false
            if(quotaTracker.Quota == 0)
            {
                return Task.FromResult(new QuotaResult() { IsAllowed = false, ResetSeconds = quotaTracker.EndTime - (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds });
            }

            // Decrement the quota
            quotaTracker.Quota--;

            // Return true
            return Task.FromResult(new QuotaResult() { IsAllowed = true, ResetSeconds = quotaTracker.EndTime - (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds });
        }

        public Task<string> GetIdentifierAsync(HttpContext context)
        {
            // Get the IP address
            var ipAddress = context.Connection.RemoteIpAddress.ToString();

            // Return the IP address
            return Task.FromResult(ipAddress);
        }
    }
}