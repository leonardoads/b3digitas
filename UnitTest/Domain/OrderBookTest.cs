using Domain;

namespace UnitTest.Domain;

public class OrderBookDataTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldInitializeProperties()
    {
        // Arrange
        string timestamp = "223412345";
        string microtimestamp = "123456789";
        List<List<string>> bids = [[ "100.5", "1" ], [ "101.0", "2" ] ];
        List<List<string>> asks = [[ "102.0", "1" ], [ "103.0", "3" ] ];

        // Act
        OrderBookData orderBookData = new(timestamp, microtimestamp, bids, asks);

        // Assert
        Assert.Equal(timestamp, orderBookData.Timestamp);
        Assert.Equal(microtimestamp, orderBookData.Microtimestamp);
        Assert.Equal(bids, orderBookData.Bids);
        Assert.Equal(asks, orderBookData.Asks);
        Assert.Equal(100.75m, orderBookData.AverageBids); // (100.5 + 101.0) / 2
        Assert.Equal(102.5m, orderBookData.AveragePriceAsks); // (102.0 + 103.0) / 2
        Assert.Equal(3m, orderBookData.TotalAmmountBids); // 1 + 2
        Assert.Equal(4m, orderBookData.TotalAmmountAsks); // 1 + 3
    }

    [Fact]
    public void Constructor_WithEmptyBidsAndAsks_ShouldInitializeToZero()
    {
        // Arrange
        string timestamp = "123456789";
        string microtimestamp = "123456789234523";
        List<List<string>> bids = [];
        List<List<string>> asks = [];

        // Act
        OrderBookData orderBookData = new(timestamp, microtimestamp, bids, asks);

        // Assert
        Assert.Equal(0m, orderBookData.AverageBids);
        Assert.Equal(0m, orderBookData.AveragePriceAsks);
        Assert.Equal(0m, orderBookData.TotalAmmountBids);
        Assert.Equal(0m, orderBookData.TotalAmmountAsks);
    }

    [Fact]
    public void Constructor_WithInvalidDecimalValues_ShouldThrowFormatException()
    {
        // Arrange
        string timestamp = "223412345";
        string microtimestamp = "123456789";
        List<List<string>> bids = [["invalid", "1"]];
        List<List<string>> asks = [["102.0", "1"]];

        // Act & Assert
        Assert.Throws<FormatException>(() => new OrderBookData(timestamp, microtimestamp, bids, asks));
    }
}
