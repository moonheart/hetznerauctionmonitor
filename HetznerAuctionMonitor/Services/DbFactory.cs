using AutoCtor;
using HetznerAuctionMonitor.Options;
using Injectio.Attributes;
using LiteDB;
using Microsoft.Extensions.Options;

namespace HetznerAuctionMonitor.Services;

[RegisterSingleton]
[AutoConstruct]
public partial class DbFactory
{
    private readonly IOptions<MonitorOption> _monitorOption;
    
    public ILiteDatabase Get()
    {
        return new LiteDatabase(_monitorOption.Value.LiteDb);
    }
}