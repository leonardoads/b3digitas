using Domain;

namespace UnitTest.Domain;


public class PriceDataTests
{
    [Fact]
    public void Constructor_ShouldInitializeProperties()
    {
        // Arrange
        decimal highestPrice = 150.75m;
        decimal lowestPrice = 100.25m;
        decimal averagePrice = 125.50m;
        decimal averageQuantity = 10.0m;

        // Act
        PriceData priceData = new(highestPrice, lowestPrice, averagePrice, averageQuantity);

        // Assert
        Assert.Equal(highestPrice, priceData.HighestPrice);
        Assert.Equal(lowestPrice, priceData.LowestPrice);
        Assert.Equal(averagePrice, priceData.AveragePrice);
        Assert.Equal(averageQuantity, priceData.AverageQuantity);
    }

}

