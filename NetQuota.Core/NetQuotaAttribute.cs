namespace NetQuota.Core {
    public class NetQuotaAttribute : Attribute  {

        public string ResourceKey {get; private set;}

        public NetQuotaAttribute(string resourceKey) {
            this.ResourceKey = resourceKey;
        }
    }
}
