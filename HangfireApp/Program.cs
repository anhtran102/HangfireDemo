using Hangfire;
using Hangfire.SqlServer;
using HangfireApp;
using HangfireApp.Interfaces;
using HangfireApp.Services;

var builder = WebApplication.CreateBuilder(args);
// REGISTER SERVICES HERE
// Add services to the container.
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
//builder.Services.AddRazorPages();
builder.Services.AddControllers();

// Add Hangfire services.
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage( builder.Configuration.GetConnectionString("DBConnection"), new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));

builder.Services.AddOptions();

builder.Services.AddHangfireServer();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddScoped<IServiceBus, ServiceBus>();
builder.Services.AddScoped<IJobTestService, JobTestService>();
//builder.Services.AddHostedService<new MessageHandler("") > ();
Random random = new Random();
var number = random.Next(100);
string instanceName = "Hangfire#3";
Console.WriteLine($"Created instance: {instanceName}");
builder.Services.AddHostedService<MessageHandler>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// REGISTER MIDDLEWARE HERE
// Configure the HTTP request pipeline.
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

//app.MapRazorPages();
app.UseHangfireDashboard();
app.MapControllers();

app.Run();
