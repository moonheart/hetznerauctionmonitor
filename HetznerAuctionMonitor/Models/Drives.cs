namespace HetznerAuctionMonitor.Models;

public class Drives
{
    public object[] selected { get; set; }
    public Hdd hdd { get; set; }
    public Sata sata { get; set; }
    public Nvme nvme { get; set; }
    public General general { get; set; }
}