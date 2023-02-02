using NetQuota;
using NetQuota.Implementations;
using NetQuota.Core.Services;
using NetQuota.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IQuotaStoreService, RedisQuotaStoreService>();
builder.Services.AddSingleton<IQuotaIdentifierService, QuotaIdentifierService>();
builder.Services.AddSingleton<IQuotaProfileService, QuotaProfileService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Using netquota middleware
app.UseNetQuota();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

