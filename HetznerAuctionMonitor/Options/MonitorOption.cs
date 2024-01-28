namespace HetznerAuctionMonitor.Options;

public class MonitorOption
{
    public string LiveDataUrl { get; set; } = "https://www.hetzner.com/_resources/app/jsondata/live_data_sb.json";
    public string MonitorCron { get; set; } = "0 0/5 * * * ? *";
    public string LiteDb { get; set; } = "Filename=HetznerAuctionMonitor.db";
    public string TelegramBotToken { get; set; } = "";
    public string TelegramChannel { get; set; } = "";
    public Dictionary<string, int> CpuPassmarks { get; set; } = new();
}