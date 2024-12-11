using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Domain;

namespace Infrastructure.Service;

public class BitStampService : IBitStampService
{
    private ClientWebSocket _client;
    public event Func<OrderBookEvent, Task>? OnMessageReceived;

    public BitStampService()
    {
        _client = new ClientWebSocket();
    }


    public async Task ConnectAsync()
    {
        await _client.ConnectAsync(new Uri("wss://ws.bitstamp.net"), CancellationToken.None);
        await Subscribe("btcusd");
        await Subscribe("ethusd");
    }

    private async Task Subscribe(string instrument)
    {
        string channel = $"{{\"event\": \"bts:subscribe\", \"data\": {{ \"channel\": \"diff_order_book_{instrument}\"}}}}";
        byte[] bytes = Encoding.UTF8.GetBytes(channel);
        ArraySegment<byte> arraySegment = new(bytes, 0, bytes.Length);
        await _client.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public async Task ReceiveMessages()
    {
        JsonSerializerOptions options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        ArraySegment<byte> buffer = new(new byte[1024 * 16]);
        while (_client.State == WebSocketState.Open)
        {
            WebSocketReceiveResult result;
            using MemoryStream ms = new();
            do
            {
                result = await _client.ReceiveAsync(buffer, default);
                ms.Write(buffer.Array!, buffer.Offset, result.Count);
            } while (!result.EndOfMessage);

            ms.Seek(0, SeekOrigin.Begin);

            using StreamReader streamReader = new(ms);
            string jsonString = streamReader.ReadToEnd();
            jsonString = jsonString.Replace("{}", "null");
            OrderBookEvent? orderBookEvent = JsonSerializer.Deserialize<OrderBookEvent>(jsonString!, options);

            if (OnMessageReceived != null && orderBookEvent?.Data != null)
            {
                await OnMessageReceived?.Invoke(orderBookEvent!)!;
            }

        }
    }

}
