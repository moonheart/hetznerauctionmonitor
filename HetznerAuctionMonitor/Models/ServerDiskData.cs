namespace HetznerAuctionMonitor.Models;

public class ServerDiskData
{
    public int[] nvme { get; set; }
    public int[] sata { get; set; }
    public int[] hdd { get; set; }
    public int[] general { get; set; }
}