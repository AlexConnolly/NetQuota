# NetQuota
Allows you to implement lightweight quota on top of your .NET application.

Start by adding the quota store and identifier services in your Program.cs:

```
// Used to fetch quotas (Definitely don't use this in production)
builder.Services.AddSingleton<IQuotaStoreService, LocalQuotaStoreService>();

// Used to identify a unique visitor (You should implement your own)
builder.Services.AddSingleton<IQuotaIdentifierService, QuotaIdentifierService>();

var app = builder.Build();
```

Make sure to use a singleton if you wish for the object to persist.

Next, add the middleware:

```
var app = builder.Build();

app.UseNetQuota();
```

After that, you can add the NetQuota attribute to any of your .NET web API routes:

```
[HttpGet(Name = "GetWeatherForecast")]
[NetQuota("WeatherForecast", 5, 60)]
```

The above example uses the "WeatherForecast" key (Note you can use the same key if you want to limit on the same resource) and sets the quota at 5 requests per 60 seconds.
