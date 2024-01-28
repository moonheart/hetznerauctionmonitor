namespace HetznerAuctionMonitor.Models;

public class Server
{
    public int id { get; set; }
    public int key { get; set; }
    public string name { get; set; }
    public string[] description { get; set; }
    public string[] information { get; set; }
    public string category { get; set; }
    public int cat_id { get; set; }
    public string cpu { get; set; }
    public int cpu_count { get; set; }
    public bool is_highio { get; set; }
    public string traffic { get; set; }
    public int bandwidth { get; set; }
    public string[] ram { get; set; }
    public int ram_size { get; set; }
    public int price { get; set; }
    public int setup_price { get; set; }
    public string[] hdd_arr { get; set; }
    public string[] hdd_hr { get; set; }
    public int hdd_size { get; set; }
    public int hdd_count { get; set; }
    public ServerDiskData serverDiskData { get; set; }
    public bool is_ecc { get; set; }
    public string datacenter { get; set; }
    public string datacenter_hr { get; set; }
    public string[] specials { get; set; }
    public string[] dist { get; set; }
    public bool fixed_price { get; set; }
    public int next_reduce { get; set; }
    public bool next_reduce_hr { get; set; }
    public int next_reduce_timestamp { get; set; }
}