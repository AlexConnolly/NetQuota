namespace NetQuota {
    public static class NetQuotaMiddlewareExtensions
    {
        public static IApplicationBuilder UseNetQuota(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<NetQuotaMiddleware>();
        }
    }
}