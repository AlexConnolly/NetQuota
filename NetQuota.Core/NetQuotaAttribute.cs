namespace NetQuota.Core {
    public class NetQuotaAttribute : Attribute  {

        public string Key {get; private set;}
        public int Amount {get; private set;}
        public int Seconds {get; private set;}

        public NetQuotaAttribute(string key, int amount, int seconds) {
            this.Key = key;
            this.Amount = amount;
            this.Seconds = seconds;
        }
    }
}
