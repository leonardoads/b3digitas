using Domain;

namespace Infrastructure.Repository;

public interface IDatabaseContext
{
    Task InsertOrderBookAsync(OrderBookEvent orderBook);
    Task<OrderBookEvent> GetLastOrderBooksAsync(string channel);
    Task InsertQuoteOrderOperation(QuoteResult quote);
}

