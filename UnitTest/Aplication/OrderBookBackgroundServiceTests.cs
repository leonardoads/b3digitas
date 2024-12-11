using Application;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTest.Application
{
    public class OrderBookBackgroundServiceTests
    {
        [Fact]
        public async Task ExecuteAsync_ShouldCallStartAsync()
        {
            // Arrange
            Mock<ILogger<OrderBookBackgroundService>> mockLogger = new();
            Mock<IOrderBookUseCase> mockOrderBookUseCase = new();
            OrderBookBackgroundService backgroundService = new(mockLogger.Object, mockOrderBookUseCase.Object);

            // Act
            await backgroundService.StartAsync(CancellationToken.None); // StartAsync is called to trigger ExecuteAsync

            // Assert
            mockOrderBookUseCase.Verify(useCase => useCase.StartAsync(), Times.Once);
        }
    }
}
