using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Timers;
using DnsClient.Internal;
using Domain;
using Infrastructure.Repository;
using Infrastructure.Service;
using Microsoft.Extensions.Logging;

namespace Application;

public interface IOrderBookUseCase
{
    Task StartAsync();
    Task SaveOrderBook(OrderBookEvent orderBook);

    Task<QuoteResult> QuoteOrderOperation(QuoteRequest quoteRequest);
}

public class OrderBookUseCase(
    IBitStampService webSocketClient,
    IDatabaseContext databaseContext,
    ILogger<OrderBookUseCase> logger) : IOrderBookUseCase
{
    private readonly IBitStampService _webSocketClient = webSocketClient;
    private readonly IDatabaseContext _databaseContext = databaseContext;
    private readonly ILogger<OrderBookUseCase> _logger = logger;
    private readonly ConcurrentDictionary<string, List<OrderBookData>> _orderBooks = [];

    private readonly System.Timers.Timer _timer = new (5_000);

    public async Task StartAsync()
    {
        await _webSocketClient.ConnectAsync();
        _webSocketClient.OnMessageReceived += async (x) => 
        {
            Console.WriteLine(x.Event);
            await Task.CompletedTask;
        };
        this._webSocketClient.OnMessageReceived += SaveOrderBook;
        this._webSocketClient.OnMessageReceived += SaveRecentOrderBook;
        this._timer.Elapsed += (sender, args) => CalculateLast5Seconds(sender!, args);
        this._timer.AutoReset = true;
        this._timer.Enabled = true;
        this._timer.Start();
        await this._webSocketClient.ReceiveMessages();
    }

    public async Task SaveOrderBook(OrderBookEvent orderBook)
    {
        await this._databaseContext.InsertOrderBookAsync(orderBook);
    }

    public async Task SaveRecentOrderBook(OrderBookEvent orderBook)
    {
        List<OrderBookData> data = this._orderBooks.GetValueOrDefault(orderBook.Channel, []);
        data.Add(orderBook.Data!);
        
        this._orderBooks[orderBook.Channel] =  data;

        await Task.CompletedTask;
    }

    private void CalculateLast5Seconds(object sender, ElapsedEventArgs e)
    {
        FrozenDictionary<string, List<OrderBookData>> lastOrders = this._orderBooks.ToFrozenDictionary();
        this._orderBooks.Clear();
        
        foreach (string channel in lastOrders.Keys)
        {
            List<OrderBookData> lastorderByChanel = lastOrders[channel];
            OrderBookData lastOrderBook = lastorderByChanel.Last();
            lastOrderBook.Asks.Sort((x, y) => x[0].CompareTo(y[0]));
            string min = lastOrderBook.Asks.First()[0];
            string max = lastOrderBook.Asks.Last()[0];
            
            decimal avgPriceAsksLast5Seconds =  lastorderByChanel.Average((orderbook) => orderbook.AveragePriceAsks);
            decimal avgTotalAmountLast5Seconds = lastorderByChanel.Average(ordebook => ordebook.TotalAmmountAsks);

            this._logger.LogInformation("Channel {channel}. Min {min} Max {max} Avg {average} Average last 5 seconds {avgPriceAsksLast5Seconds} Total Amount Last 5s {avgTotalAmountLast5Seconds}",
                channel, min, max, lastOrderBook.AveragePriceAsks, avgPriceAsksLast5Seconds, avgTotalAmountLast5Seconds);
        }

    }

    public async Task<QuoteResult> QuoteOrderOperation(QuoteRequest quoteRequest)
    {
        string channel = $"diff_order_book_{quoteRequest.Instrument}";
        OrderBookEvent lastOrderBook = await this._databaseContext.GetLastOrderBooksAsync(channel);

        List<List<string>> operationData = quoteRequest.Operation switch
        {
            "bids" => [.. lastOrderBook.Data!.Bids.OrderByDescending(x => decimal.Parse(x[0]))],
            "asks" => [.. lastOrderBook.Data!.Asks.OrderBy(x => decimal.Parse(x[0]))],
            _ => throw new InvalidOperationException()
        };
        QuoteResult quote = CalculateQuoteResult(quoteRequest, operationData);

        await this._databaseContext.InsertQuoteOrderOperation(quote);

        return quote;

    }

    private static QuoteResult CalculateQuoteResult(QuoteRequest simulateOrder, List<List<string>> operationData)
    {
        decimal totalAmount = 0;
        decimal quotedPrice = 0;
        int iterator = 0;
        List<List<string>> quoteItems = [];
        while (totalAmount < simulateOrder.Quantity && iterator < operationData.Count)
        {
            List<string> item = operationData[iterator];
            decimal price = decimal.Parse(item[0]);
            decimal amount = decimal.Parse(item[1]);
            totalAmount += amount;
            quotedPrice += price * amount;
            quoteItems.Add(item);

            iterator += 1;
        }

        QuoteResult quote = new(quoteItems,
                                simulateOrder.Quantity,
                                simulateOrder.Operation,
                                quotedPrice);
        return quote;
    }
}
