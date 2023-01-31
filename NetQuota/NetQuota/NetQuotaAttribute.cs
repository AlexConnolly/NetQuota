namespace NetQuota {
    public class NetQuotaAttribute : Attribute  {

        private readonly string _key;
        private readonly int _quota;
        private readonly int _period;

        public string Key => _key;
        public int Quota => _quota;
        public int Period => _period;

        public NetQuotaAttribute(string key, int quota, int period) {
            this._key = key;
            this._quota = quota;
            this._period = period;
        }
    }
}
