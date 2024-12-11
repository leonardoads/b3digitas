namespace Domain;

public class QuoteResult
{
    public List<List<string>> QuoteItems { get; }
    public int Quantity { get; }
    public string Operation { get; }
    public decimal QuotedPrice { get; }
    public string Identifier { get; }
    public DateTime CreatedAt { get; }

    public QuoteResult(List<List<string>> quoteItems,
                       int quantity,
                       string operation,
                       decimal quotedPrice,
                       string identifier = "",
                       DateTime createdAt = default)
    {
        QuoteItems = quoteItems;
        Quantity = quantity;
        Operation = operation;
        QuotedPrice = quotedPrice;
        Identifier = identifier != string.Empty ? identifier : Guid.NewGuid().ToString();
        CreatedAt = createdAt != default ? createdAt : DateTime.UtcNow;
    }

}