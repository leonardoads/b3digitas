namespace Infrastructure.Repository;

using Domain;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Threading.Tasks;

public class DatabaseContext : IDatabaseContext
{
    private readonly IMongoDatabase _database;

    public DatabaseContext(IConfiguration configuration)
    {
        MongoClient client = new(configuration.GetConnectionString("MongoDb"));
        _database = client.GetDatabase("OrderBookDb");
    }

    public async Task InsertOrderBookAsync(OrderBookEvent orderBook)
    {

        CreateIndexModel<OrderBookEvent> indexModel = new(
            Builders<OrderBookEvent>.IndexKeys.Descending("CreatedAt"),
            new CreateIndexOptions
            {
                ExpireAfter = TimeSpan.FromSeconds(1000),
                Name = "ExpireAtIndex"
            }
        );
        IMongoCollection<OrderBookEvent> collection = _database.GetCollection<OrderBookEvent>("OrderBooks");

        //No mundo real o controle de criar os indices direto no banco é mais plausivel
        //Porém a titulo de manter o container do mongo sem tantos documentos, e o 
        // fato de que a criação de indices é uma operação indepontente, optei por manter
        // esse controle aqui. 
        await collection.Indexes.CreateOneAsync(indexModel);

        await collection.InsertOneAsync(orderBook);
    }

    public async Task<OrderBookEvent> GetLastOrderBooksAsync(string channel)
    {
        IMongoCollection<OrderBookEvent> collection = _database.GetCollection<OrderBookEvent>("OrderBooks");
        ProjectionDefinition<OrderBookEvent> projection = Builders<OrderBookEvent>.Projection.Exclude("_id");

        return  await collection
                        .Find(orderbook => orderbook.Channel == channel)
                        .Project<OrderBookEvent>(projection)
                        .SortByDescending(orderBook => orderBook.CreatedAt)
                        .FirstAsync();
    }

    public async Task InsertQuoteOrderOperation(QuoteResult quote)
    {
        IMongoCollection<QuoteResult> collection = _database.GetCollection<QuoteResult>("QuoteOrders");

        await collection.InsertOneAsync(quote);
    }
}

