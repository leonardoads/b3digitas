using Domain;

namespace Infrastructure.Service;

public interface IBitStampService
{
    public event Func<OrderBookEvent, Task>? OnMessageReceived;

    Task ConnectAsync();

    Task ReceiveMessages();
}
