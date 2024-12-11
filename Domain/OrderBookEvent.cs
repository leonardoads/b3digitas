namespace Domain;

public class OrderBookEvent
{
    public OrderBookData? Data { get; }
    public string Channel { get; }
    public string Event { get; }
    public DateTime CreatedAt {get; }

    public OrderBookEvent(OrderBookData? data,
                          string channel,
                          string @event,
                          DateTime createdAt = default)
    {
        Data = data;
        Channel = channel;
        Event = @event;
        CreatedAt = createdAt != default ? createdAt : DateTime.UtcNow;
    }
}