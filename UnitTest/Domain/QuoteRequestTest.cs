using Domain;

namespace UnitTest.Domain
{
    public class QuoteRequestTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange
            string operation = "buy";
            string instrument = "AAPL";
            int quantity = 10;

            // Act
            QuoteRequest quoteRequest = new(operation, instrument, quantity);

            // Assert
            Assert.Equal(operation, quoteRequest.Operation);
            Assert.Equal(instrument, quoteRequest.Instrument);
            Assert.Equal(quantity, quoteRequest.Quantity);
        }
    }
}
