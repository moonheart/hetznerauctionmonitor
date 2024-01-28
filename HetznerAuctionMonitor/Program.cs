using System.Diagnostics;
using HetznerAuctionMonitor;
using HetznerAuctionMonitor.Bot;
using HetznerAuctionMonitor.Options;
using Quartz;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddJsonFile("appsettings.user.json", true, false);
builder.Services.AddOptions();
builder.Services.Configure<MonitorOption>(builder.Configuration.GetSection("Monitor"));
builder.Services.AddQuartz(q =>
{
    var monitorOption = builder.Configuration.GetSection("Monitor").Get<MonitorOption>();
    if (monitorOption == null)
    {
        throw new Exception("MonitorOption is null");
    }

    if (Debugger.IsAttached)
    {
        monitorOption.MonitorCron = "* * * * * ? *";
    }
    q.ScheduleJob<MonitorJob>(configurator => configurator.WithCronSchedule(monitorOption.MonitorCron));
});

builder.Services.AddQuartzHostedService(opt => { opt.WaitForJobsToComplete = true; });
builder.Services.AddHttpClient();
builder.Services.AddTelegramBot();
builder.Services.AddHetznerAuctionMonitor();

var host = builder.Build();
host.Run();