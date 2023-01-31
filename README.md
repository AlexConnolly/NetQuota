# NetQuota
Allows you to implement lightweight quota on top of your .NET application.

Start by adding the quota service (We can use LocalQuotaService as an example) in your Program.cs:

```
builder.Services.AddSingleton<IQuotaService, LocalQuotaService>();

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
