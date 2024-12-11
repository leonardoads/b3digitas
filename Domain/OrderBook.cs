namespace Domain;

public class OrderBookData
{
    public string Timestamp { get; }
    public string Microtimestamp { get; }
    public List<List<string>> Bids { get; }
    public List<List<string>> Asks { get; }

    public decimal AveragePriceAsks { get; }

    public decimal AverageBids {get;}
    public decimal TotalAmmountAsks { get; }

    public decimal TotalAmmountBids {get;}

    public OrderBookData(string timestamp, string microtimestamp, List<List<string>> bids, List<List<string>> asks)
    {
        Timestamp = timestamp;
        Microtimestamp = microtimestamp;
        Bids = bids;
        Asks = asks;
        AveragePriceAsks = Asks.Count > 0 ? Asks.Average(x => decimal.Parse(x[0])) : 0;
        AverageBids = Bids.Count > 0 ? Bids.Average(x => decimal.Parse(x[0])) : 0;
        TotalAmmountAsks = Asks.Sum(ask => decimal.Parse(ask[1]));
        TotalAmmountBids = Bids.Sum(bid => decimal.Parse(bid[1]));
    }


}