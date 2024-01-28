namespace HetznerAuctionMonitor.Models;

public class Filter
{
    public string freetext { get; set; }
    public Price price { get; set; }
    public OnlyAuctions onlyAuctions { get; set; }
    public Location location { get; set; }
    public CpuType cpuType { get; set; }
    public Additional additional { get; set; }
    public Ram ram { get; set; }
    public Ecc ecc { get; set; }
    public bool noSetupExists { get; set; }
    public NoSetup noSetup { get; set; }
    public Drives drives { get; set; }
}