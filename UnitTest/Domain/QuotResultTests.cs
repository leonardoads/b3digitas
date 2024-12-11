using Domain;

namespace UnitTest.Domain;

public class QuoteResultTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldInitializeProperties()
    {
        // Arrange
        List<List<string>> quoteItems = [["Item1", "100"], ["Item2", "200"]];
        int quantity = 5;
        string operation = "buy";
        decimal quotedPrice = 150.75m;
        string identifier = "custom-id";
        DateTime createdAt = new(2023, 10, 01, 12, 0, 0, DateTimeKind.Utc);

        // Act
        QuoteResult quoteResult = new(quoteItems, quantity, operation, quotedPrice, identifier, createdAt);

        // Assert
        Assert.Equal(quoteItems, quoteResult.QuoteItems);
        Assert.Equal(quantity, quoteResult.Quantity);
        Assert.Equal(operation, quoteResult.Operation);
        Assert.Equal(quotedPrice, quoteResult.QuotedPrice);
        Assert.Equal(identifier, quoteResult.Identifier);
        Assert.Equal(createdAt, quoteResult.CreatedAt);
    }

    [Fact]
    public void Constructor_WithDefaultIdentifier_ShouldGenerateNewGuid()
    {
        // Arrange
        List<List<string>> quoteItems = [["Item1", "100"]];
        int quantity = 5;
        string operation = "buy";
        decimal quotedPrice = 150.75m;

        // Act
        QuoteResult quoteResult = new(quoteItems, quantity, operation, quotedPrice);

        // Assert
        Assert.NotNull(quoteResult.Identifier);
        Assert.False(string.IsNullOrEmpty(quoteResult.Identifier));
        Assert.True(Guid.TryParse(quoteResult.Identifier, out _)); // Check if it's a valid GUID
    }

    [Fact]
    public void Constructor_WithDefaultCreatedAt_ShouldSetToUtcNow()
    {
        // Arrange
        List<List<string>> quoteItems = [["Item1", "100"]];
        int quantity = 5;
        string operation = "buy";
        decimal quotedPrice = 150.75m;

        // Act
        QuoteResult quoteResult = new(quoteItems, quantity, operation, quotedPrice);

        // Assert
        Assert.True(quoteResult.CreatedAt <= DateTime.UtcNow && quoteResult.CreatedAt >= DateTime.UtcNow.AddSeconds(-1));
    }

    [Fact]
    public void Constructor_WithEmptyIdentifier_ShouldGenerateNewGuid()
    {
        // Arrange
        List<List<string>> quoteItems = [["Item1", "100"]];
        int quantity = 5;
        string operation = "buy";
        decimal quotedPrice = 150.75m;
        string identifier = string.Empty; // Empty identifier

        // Act
        QuoteResult quoteResult = new(quoteItems, quantity, operation, quotedPrice, identifier);

        // Assert
        Assert.NotNull(quoteResult.Identifier);
        Assert.False(string.IsNullOrEmpty(quoteResult.Identifier));
        Assert.True(Guid.TryParse(quoteResult.Identifier, out _)); // Check if it's a valid GUID
    }
}


