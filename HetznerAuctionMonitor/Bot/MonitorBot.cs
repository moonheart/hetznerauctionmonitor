using System.Text;
using AutoCtor;
using HetznerAuctionMonitor.Models;
using HetznerAuctionMonitor.Options;
using Injectio.Attributes;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Extensions.Markup;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace HetznerAuctionMonitor.Bot;

[RegisterSingleton]
[AutoConstruct]
public partial class MonitorBot
{
    private readonly ITelegramBotClient _bot;
    private readonly IOptions<MonitorOption> _monitorOption;
    private readonly ILogger<MonitorOption> _logger;

    [AutoPostConstruct]
    private void Initialize()
    {
        try
        {
            // await _bot.SendTextMessageAsync(_monitorOption.Value.TelegramChannel, "Bot started");
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }
    }

    public async Task NewServer(Server server)
    {
        var msg = $"""
                   🆕 New server found:
                   ID: `{server.id}`
                   CPU: `{Escape(server.cpu)}`
                   RAM: {server.ram_size} GB
                   Price: {(server.fixed_price ? "🔒 " : "")}€{server.price} monthly
                   Expires: {FormatTime(server.next_reduce)} left

                   {string.Join("\n", server.information.Select(Escape))}

                   \#{Escape(server.datacenter)}

                   Open the [server auction page](https://www.hetzner.com/sb?country=ot) and type the ID in the search box to find the details\.
                   """;
        await _bot.SendTextMessageAsync(_monitorOption.Value.TelegramChannel, msg, parseMode: ParseMode.MarkdownV2);
    }

    private static string Escape(string msg)
    {
        return Tools.EscapeMarkdown(msg, ParseMode.MarkdownV2);
    }

    public async Task ServerPriceChanged(Server oldServer, Server newServer)
    {
        if (oldServer.price != newServer.price)
        {
            var msg = $"""
                       📉 Server price changed:
                       ID: `{newServer.id}`
                       CPU: `{Escape(newServer.cpu)}`
                       RAM: {newServer.ram_size} GB
                       Price: {oldServer.price} \=\> {(newServer.fixed_price ? "🔒" : "")}€{newServer.price} monthly
                       Expires: {FormatTime(newServer.next_reduce)} left

                       {string.Join("\n", newServer.information.Select(Escape))}

                       \#{Escape(newServer.datacenter)}

                       Open the [server auction page](https://www.hetzner.com/sb?country=ot) and type the ID in the search box to find the details\.
                       """;
            await _bot.SendTextMessageAsync(new ChatId(_monitorOption.Value.TelegramChannel), msg, parseMode: ParseMode.MarkdownV2);
        }
    }

    private string FormatTime(long time)
    {
        var timeSpan = TimeSpan.FromSeconds(time);
        return Escape(new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds).ToString());
    }

    public async Task UpdateServerStats(Server[] servers)
    {
        var maxCpu = servers.Max(s => s.cpu.Replace("AMD ", "").Replace("Intel ", "").Length);
        var tableHeader = $"""
                           ```
                           |{"cpu".PadRight(maxCpu)}|cnt |price  |score |rank |
                           |{"----".PadRight(maxCpu, '-')}|----|-------|------|-----|
                           """;
        var sb = new StringBuilder();
        sb.AppendLine(Escape($"Stats updated: {DateTimeOffset.Now}"));
        sb.AppendLine(Escape($"Score is Passmark score. Rank is score/min price."));
        sb.AppendLine(tableHeader);
        var cpuPassmarks = _monitorOption.Value.CpuPassmarks;
        foreach (var x1 in servers.GroupBy(d => d.cpu).Select(grouping =>
                 {
                     var minPrice = grouping.Min(s => s.price);
                     var maxPrice = grouping.Max(s => s.price);
                     var priceStr = minPrice == maxPrice ? $"{minPrice}" : $"{minPrice}~{maxPrice}";
                     var score = cpuPassmarks.GetValueOrDefault(grouping.Key, 0);
                     var rank = score / minPrice;
                     return new
                     {
                         cpu = grouping.Key.Replace("AMD ", "").Replace("Intel ", ""),
                         count = grouping.Count(),
                         price = priceStr,
                         score,
                         rank
                     };
                 }).OrderByDescending(d => d.rank))
        {
            sb.AppendLine($"|{x1.cpu.PadRight(maxCpu)}|{x1.count.ToString().PadRight(4)}|{x1.price.PadRight(7)}|{x1.score.ToString().PadRight(6)}|{x1.rank.ToString().PadRight(5)}|");
        }

        sb.AppendLine("```");

        var msg = sb.ToString();

        var replyMarkup = new InlineKeyboardMarkup(new InlineKeyboardButton("Go Auction") {Url = "https://www.hetzner.com/sb?country=ot"});
        var chat = await _bot.GetChatAsync(_monitorOption.Value.TelegramChannel);
        if (chat.PinnedMessage != null)
        {
            await _bot.EditMessageTextAsync(
                _monitorOption.Value.TelegramChannel,
                chat.PinnedMessage.MessageId,
                msg,
                parseMode: ParseMode.MarkdownV2,
                replyMarkup: replyMarkup);
        }
        else
        {
            var sentMessage = await _bot.SendTextMessageAsync(
                _monitorOption.Value.TelegramChannel,
                msg,
                parseMode: ParseMode.MarkdownV2,
                replyMarkup: replyMarkup);
            await _bot.PinChatMessageAsync(_monitorOption.Value.TelegramChannel, sentMessage.MessageId);
        }
    }
}