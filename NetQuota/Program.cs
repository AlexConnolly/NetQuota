using NetQuota;
using NetQuota.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add the quota service
builder.Services.AddSingleton<IQuotaService, LocalQuotaService>();

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

