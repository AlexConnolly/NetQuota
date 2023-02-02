# NetQuota
Allows you to implement lightweight quotas on top of your .NET application.

Start by adding the quota store and identifier services in your Program.cs:

```
// Used to fetch quotas (Definitely don't use this in production)
builder.Services.AddSingleton<IQuotaStoreService, LocalQuotaStoreService>();

// Used to identify a unique visitor (You should implement your own)
builder.Services.AddSingleton<IQuotaIdentifierService, QuotaIdentifierService>();

// Used to define the quotas for resources
builder.Services.AddSingleton<IQuotaProfileService, QuotaProfileService>();

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
[NetQuota("Weather")]
```

```
public class QuotaProfileService : IQuotaProfileService
{
    public QuotaProfile GetDefaultProfile()
    {
        return new QuotaProfile("Default", new List<QuotaDefinition>()
        {
            new QuotaDefinition("Weather", 60, 5)
        });
    }

    public QuotaProfile GetProfileForIdentifier(string identifier)
    {
        switch(identifier)
        {
            case "Enterprise":
                return new QuotaProfile("Enterprise", new List<QuotaDefinition>()
                {
                    new QuotaDefinition("Weather", 60, 10)
                });

            default:
                // We can return null as we have a default profile 
                return null;

        }
    }
}
```

The above example uses the "Weather" resource. This is defined in the QuotaProfileService. By default, we get 5 requests per 60 seconds. If your identifier is "Enterprise", however, you are allowed 10 per 60 seconds (This is a very bad implementation and instead I would recommend storing these values in the encrypted token).
