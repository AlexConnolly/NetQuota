using NetQuota.Core;
using NetQuota.Core.Services;
using StackExchange.Redis;

namespace NetQuota.Implementations
{
    public class RedisQuotaStoreService : IQuotaStoreService
    {
        private ConnectionMultiplexer redis;
        public RedisQuotaStoreService() {

            // Todo: Fix this so that we don't rely on this
            this.redis = ConnectionMultiplexer.Connect(new ConfigurationOptions() {
                AbortOnConnectFail = false,
                EndPoints = { "" }, 
                User = "",
                Password = ""
            });
        }

        public async Task<QuotaInstance> GetQuotaAsync(string identifier, string key)
        {
            // Create a redis connection
            var db = redis.GetDatabase();

            var value = db.StringGet(identifier + ":" + key);

            if(value.HasValue) {
                string valueString = value.ToString();

                int amountLeft = int.Parse(valueString.Split('@')[0]);
                DateTime expiresOn = DateTime.Parse(valueString.Split('@')[1]);

                return new QuotaInstance() { AmountLeft = amountLeft, ExpiresOn = expiresOn };
            } else {
                return null;
            }
        }

        public async Task SetQuotaAsync(string identifier, string key, QuotaInstance quota)
        {
            // Create a redis connection
            var db = redis.GetDatabase();

            db.StringSet(identifier + ":" + key, quota.AmountLeft + "@" + quota.ExpiresOn.ToString());
        }
    }
}