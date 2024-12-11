using Domain;

namespace UnitTest.Domain;
public class OrderBookEventTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldInitializeProperties()
    {
        // Arrange
        OrderBookData orderBookData = new(
            "2023-10-01T12:00:00Z",
            "123456789",
            [["100.5", "1"]],
            [["102.0", "1"]]
        );
        string channel = "order_book_channel";
        string eventType = "update";
        DateTime createdAt = new(2023, 10, 1, 12, 0, 0, DateTimeKind.Utc);

        // Act
        OrderBookEvent orderBookEvent = new(orderBookData, channel, eventType, createdAt);

        // Assert
        Assert.Equal(orderBookData, orderBookEvent.Data);
        Assert.Equal(channel, orderBookEvent.Channel);
        Assert.Equal(eventType, orderBookEvent.Event);
        Assert.Equal(createdAt, orderBookEvent.CreatedAt);
    }

    [Fact]
    public void Constructor_WithDefaultCreatedAt_ShouldSetToUtcNow()
    {
        // Arrange
        OrderBookData orderBookData = new(
            "3453454534",
            "345345453453",
            [["100.5", "1"]],
            [["102.0", "1"]]
        );
        string channel = "order_book_channel";
        string eventType = "update";

        // Act
        OrderBookEvent orderBookEvent = new(orderBookData, channel, eventType);

        // Assert
        Assert.Equal(orderBookData, orderBookEvent.Data);
        Assert.Equal(channel, orderBookEvent.Channel);
        Assert.Equal(eventType, orderBookEvent.Event);
        Assert.True(orderBookEvent.CreatedAt <= DateTime.UtcNow && orderBookEvent.CreatedAt >= DateTime.UtcNow.AddSeconds(-1));
    }

    [Fact]
    public void Constructor_WithNullData_ShouldInitializeDataToNull()
    {
        // Arrange
        string channel = "order_book_channel";
        string eventType = "update";

        // Act
        OrderBookEvent orderBookEvent = new(null, channel, eventType);

        // Assert
        Assert.Null(orderBookEvent.Data);
        Assert.Equal(channel, orderBookEvent.Channel);
        Assert.Equal(eventType, orderBookEvent.Event);
        Assert.True(orderBookEvent.CreatedAt <= DateTime.UtcNow && orderBookEvent.CreatedAt >= DateTime.UtcNow.AddSeconds(-1));
    }
}

