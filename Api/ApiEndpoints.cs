using Application;
using Domain;

namespace Api;

public static class ApiEndpoints
{
    public static void MapApiEndpoints(this WebApplication app)
    {
        IOrderBookUseCase orderBookUseCase = app.Services.GetService<IOrderBookUseCase>()!;

        app.MapPost("/items", async (QuoteRequest quote) =>
        {
            return await orderBookUseCase.QuoteOrderOperation(quote);
        });

    }
}