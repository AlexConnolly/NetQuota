using NetQuota.Core;
using NetQuota.Core.Services;
using StackExchange.Redis;

namespace NetQuota.Implementations
{
    public class RedisQuotaStoreService : IQuotaStoreService
    {
        private ConnectionMultiplexer redis;
        public RedisQuotaStoreService() {
            this.redis = ConnectionMultiplexer.Connect(new ConfigurationOptions() {
                AbortOnConnectFail = false,
                EndPoints = {"localhost:6379"}
            });
        }

        public Task<Quota> GetQuotaAsync(string identifier, string key)
        {
            // Create a redis connection
            var db = redis.GetDatabase();

            var value = db.StringGet(identifier + ":" + key);

            if(value.HasValue) {
                string valueString = value.ToString();

                int amountLeft = int.Parse(valueString.Split('@')[0]);
                DateTime expiresOn = DateTime.Parse(valueString.Split('@')[1]);

                return Task.FromResult(new Quota() { AmountLeft = amountLeft, ExpiresOn = expiresOn});
            } else {
                return Task.FromResult<Quota>(null);
            }
        }

        public Task SetQuotaAsync(string identifier, string key, Quota quota)
        {
            // Create a redis connection
            var db = redis.GetDatabase();

            db.StringSet(identifier + ":" + key, quota.AmountLeft + "@" + quota.ExpiresOn.ToString());

            return Task.CompletedTask;
        }
    }
}