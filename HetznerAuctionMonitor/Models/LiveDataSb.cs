namespace HetznerAuctionMonitor.Models;

public class LiveDataSb
{
    public Server[] server { get; set; }
    public Filter filter { get; set; }
    public int serverCount { get; set; }
}