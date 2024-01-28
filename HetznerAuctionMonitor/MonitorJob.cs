using System.Text.Json;
using AutoCtor;
using HetznerAuctionMonitor.Bot;
using HetznerAuctionMonitor.Models;
using HetznerAuctionMonitor.Options;
using HetznerAuctionMonitor.Services;
using Microsoft.Extensions.Options;
using Quartz;
using Telegram.Bot;

namespace HetznerAuctionMonitor;

[DisallowConcurrentExecution]
[AutoConstruct]
public partial class MonitorJob : IJob
{
    private readonly MonitorBot _monitorBot;
    private readonly DbFactory _dbFactory;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<MonitorOption> _monitorOption;
    private readonly ILogger<MonitorJob> _logger;

    public async Task Execute(IJobExecutionContext context)
    {
        var liveDataUrl = _monitorOption.Value.LiveDataUrl;
        var httpClient = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, liveDataUrl);
        request.Headers.Add("Accept", "*/*");
        request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
        request.Headers.Add("Accept-Language", "en-US,en;q=0.9");
        request.Headers.Add("Referer", "https://www.hetzner.com/sb");
        request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36 Edg/120.0.0.0");
        
        var json = await httpClient.GetStringAsync($"{liveDataUrl}?m={DateTimeOffset.Now.ToUnixTimeMilliseconds()}");
        var data = JsonSerializer.Deserialize<LiveDataSb>(json);
        if (data == null)
        {
            _logger.LogWarning("LiveDataSb is null");
            return;
        }

        using (var db = _dbFactory.Get())
        {
            var col = db.GetCollection<Server>();
            foreach (var server in data.server)
            {
                var findById = col.FindOne(s => s.id == server.id);
                if (findById == null)
                {
                    // new server
                    // await _monitorBot.NewServer(server);
                    col.Insert(server);
                }
                else
                {
                    // update server
                    if (findById.price != server.price)
                    {
                        // price changedGo
                        // await _monitorBot.ServerPriceChanged(findById, server);
                    }
                    col.Update(server);
                }
            }
        }

        await _monitorBot.UpdateServerStats(data.server);
    }
}