namespace HetznerAuctionMonitor.Models;

public class Price
{
    public int min { get; set; }
    public int lowest { get; set; }
    public int max { get; set; }
    public int step { get; set; }
    public int from { get; set; }
    public int to { get; set; }
}