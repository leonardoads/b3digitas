using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application;

public class OrderBookBackgroundService : BackgroundService
{

    private readonly ILogger<OrderBookBackgroundService> _logger;
    private readonly IOrderBookUseCase _orderBookUseCase;

    public OrderBookBackgroundService(ILogger<OrderBookBackgroundService> logger, IOrderBookUseCase orderbookUseCase)
    {
        _logger = logger;
        _orderBookUseCase = orderbookUseCase;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _orderBookUseCase.StartAsync();
    }
}
