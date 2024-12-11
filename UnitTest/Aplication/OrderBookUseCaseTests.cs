using Application;
using Domain;
using Infrastructure.Repository;
using Infrastructure.Service;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTest.Application
{
    public class OrderBookUseCaseTests
    {
        [Fact]
        public async Task QuoteOrderOperation_WithOperationAsBids_ShouldCalculateQuoteResultCorrectly()
        {
            // Arrange
            Mock<IBitStampService> mockWebSocketClient = new();
            Mock<IDatabaseContext> mockDatabaseContext = new();
            Mock<ILogger<OrderBookUseCase>> mockLogger = new();
            OrderBookUseCase orderBookUseCase = new(mockWebSocketClient.Object, mockDatabaseContext.Object, mockLogger.Object);

            QuoteRequest quoteRequest = new("bids", "btcusd", 10);
            List<List<string>> bids =
            [
                ["100", "5"], 
                ["99", "5"],
                ["98", "3"] 
            ];
            OrderBookData orderBookData = new("2023-10-01T12:00:00Z", "123456789", bids, []);
            OrderBookEvent orderBookEvent = new(orderBookData, "diff_order_book_btcusd", "order_book_update");

            mockDatabaseContext.Setup(db => db.GetLastOrderBooksAsync(It.IsAny<string>())).ReturnsAsync(orderBookEvent);
            mockDatabaseContext.Setup(db => db.InsertQuoteOrderOperation(It.IsAny<QuoteResult>())).Returns(Task.CompletedTask);

            // Act
            QuoteResult result = await orderBookUseCase.QuoteOrderOperation(quoteRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(quoteRequest.Quantity, result.Quantity);
            Assert.Equal(10, result.Quantity); // Ensure the quantity matches the request
            Assert.Equal("bids", result.Operation); // Ensure the operation matches the request

            // Calculate expected quoted price
            decimal expectedQuotedPrice =  (5 * 99) + (5 * 100); 
            Assert.Equal(expectedQuotedPrice, result.QuotedPrice); // Ensure the quoted price is calculated correctly

            Assert.Equal(2, result.QuoteItems.Count); 

            Assert.Equal("100", result.QuoteItems[0][0]); // First item should be 100
            Assert.Equal("5", result.QuoteItems[0][1]); // First item amount should be 5
            Assert.Equal("99", result.QuoteItems[1][0]); // Second item should be 99
            Assert.Equal("5", result.QuoteItems[1][1]); // Second item amount should be 5

            mockDatabaseContext.Verify(db => db.InsertQuoteOrderOperation(It.IsAny<QuoteResult>()), Times.Once);
        }

        [Fact]
        public async Task QuoteOrderOperation_WithOperationAsAsks_ShouldCalculateQuoteResultCorrectly()
        {
            // Arrange
            Mock<IBitStampService> mockWebSocketClient = new();
            Mock<IDatabaseContext> mockDatabaseContext = new();
            Mock<ILogger<OrderBookUseCase>> mockLogger = new();
            OrderBookUseCase orderBookUseCase = new(mockWebSocketClient.Object, mockDatabaseContext.Object, mockLogger.Object);

            QuoteRequest quoteRequest = new("asks", "btcusd", 10);
            List<List<string>> asks =
            [
                ["100", "5"], 
                ["99", "5"],
                ["98", "5"] 
            ];
            OrderBookData orderBookData = new("1234124", "123456789", [], asks);
            OrderBookEvent orderBookEvent = new(orderBookData, "diff_order_book_btcusd", "order_book_update");

            mockDatabaseContext.Setup(db => db.GetLastOrderBooksAsync(It.IsAny<string>())).ReturnsAsync(orderBookEvent);
            mockDatabaseContext.Setup(db => db.InsertQuoteOrderOperation(It.IsAny<QuoteResult>())).Returns(Task.CompletedTask);

            // Act
            QuoteResult result = await orderBookUseCase.QuoteOrderOperation(quoteRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(quoteRequest.Quantity, result.Quantity);
            Assert.Equal(10, result.Quantity); // Ensure the quantity matches the request
            Assert.Equal("asks", result.Operation); // Ensure the operation matches the request

            // Calculate expected quoted price
            decimal expectedQuotedPrice =  (5 * 98) + (5 * 99); 
            Assert.Equal(expectedQuotedPrice, result.QuotedPrice); // Ensure the quoted price is calculated correctly

            Assert.Equal(2, result.QuoteItems.Count); 

            Assert.Equal("98", result.QuoteItems[0][0]); // First item should be 98
            Assert.Equal("5", result.QuoteItems[0][1]); // First item amount should be 5
            Assert.Equal("99", result.QuoteItems[1][0]); // Second item should be 99
            Assert.Equal("5", result.QuoteItems[1][1]); // Second item amount should be 5
            
            mockDatabaseContext.Verify(db => db.InsertQuoteOrderOperation(It.IsAny<QuoteResult>()), Times.Once);
        }
    }
}
