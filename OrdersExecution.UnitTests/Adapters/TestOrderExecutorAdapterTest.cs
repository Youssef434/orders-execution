using System.Collections.Concurrent;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using OrdersExecutionApi.Adapters;
using OrdersExecutionApi.Converters;
using OrdersExecutionApi.Models;
using OrdersExecutionApi.Services;

namespace OrdersExecution.UnitTests.Adapters;

[TestFixture]
internal sealed class TestOrderExecutorAdapterTest
{
    [Test] public async Task TestExecuteOrderWhenCacheIsEmptyAsync()
    {
        var order = new Order(Way.Buy, string.Empty, 10, 10, DateTime.Now);
        var trade = new OrdersExecutionExercise.Trade 
        {
            Way = OrdersExecutionExercise.Way.Buy, 
            Instrument = string.Empty, 
            ExecutedQuantity = 10, 
            ExecutedPrice = 10, 
            ExecutionTime = DateTime.Now.AddMinutes(1)
        };
        var convertedTrade = trade.Convert();
        var orderExecutor = new Mock<OrdersExecutionExercise.IOrderExecutor>(MockBehavior.Strict);
        orderExecutor.Setup(x => x.ExecuteOrderAsync(It.IsAny<OrdersExecutionExercise.Order>(), CancellationToken.None))
            .ReturnsAsync(trade);
        var logger = new NullLogger<IOrderExecutor>();
        var tradeCache = new TradeCache();
        var orderExecutorAdapter = new OrderExecutorAdapter(orderExecutor.Object, tradeCache, logger);

        var tradeResult = await orderExecutorAdapter.ExecuteOrderAsync(order, CancellationToken.None).ConfigureAwait(false);
        
        Assert.That(tradeResult, Is.EqualTo(convertedTrade));
        Assert.That(tradeCache.TryGetTrade(order, out var cachedTrade));
        Assert.That(cachedTrade, Is.EqualTo(convertedTrade));
        orderExecutor.VerifyAll();
    }
    [Test] public async Task TestExecuteOrderWheOrderExistsInCacheAsync()
    {
        var order = new Order(Way.Buy, string.Empty, 10, 10, DateTime.Now);
        var trade = new OrdersExecutionExercise.Trade 
        {
            Way = OrdersExecutionExercise.Way.Buy, 
            Instrument = string.Empty, 
            ExecutedQuantity = 10, 
            ExecutedPrice = 10, 
            ExecutionTime = DateTime.Now.AddMinutes(1)
        };
        var convertedTrade = trade.Convert();
        var orderExecutor = new Mock<OrdersExecutionExercise.IOrderExecutor>(MockBehavior.Strict);
        var logger = new NullLogger<IOrderExecutor>();
        var tradeCache = new TradeCache(new ConcurrentDictionary<Order, Trade>
        {
            [order] = convertedTrade
        });
        var orderExecutorAdapter = new OrderExecutorAdapter(orderExecutor.Object, tradeCache, logger);

        var tradeResult = await orderExecutorAdapter.ExecuteOrderAsync(order, CancellationToken.None).ConfigureAwait(false);
        
        Assert.That(tradeResult, Is.EqualTo(convertedTrade));
        Assert.That(tradeCache.TryGetTrade(order, out var cachedTrade));
        Assert.That(cachedTrade, Is.EqualTo(convertedTrade));
        orderExecutor.VerifyAll();
    }

    [Test] public void TestExecuteOrderWhenOperationCancelled()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(1);
        var cancellationToken = cancellationTokenSource.Token;
        var order = new Order(Way.Buy, string.Empty, 10, 10, DateTime.Now);
        var trade = new OrdersExecutionExercise.Trade 
        {
            Way = OrdersExecutionExercise.Way.Buy, 
            Instrument = string.Empty, 
            ExecutedQuantity = 10, 
            ExecutedPrice = 10, 
            ExecutionTime = DateTime.Now.AddMinutes(1)
        };
        var orderExecutor = new Mock<OrdersExecutionExercise.IOrderExecutor>(MockBehavior.Strict);
        orderExecutor.Setup(x => x.ExecuteOrderAsync(It.IsAny<OrdersExecutionExercise.Order>(), cancellationToken))
            .ReturnsAsync(trade)
            .Callback(() => throw new OperationCanceledException("Task was cancelled."));
        var logger = new NullLogger<IOrderExecutor>();
        var tradeCache = new TradeCache();
        var orderExecutorAdapter = new OrderExecutorAdapter(orderExecutor.Object, tradeCache, logger);
        Assert.ThrowsAsync<OperationCanceledException>(async () =>
            await orderExecutorAdapter.ExecuteOrderAsync(order, cancellationToken).ConfigureAwait(false));
        orderExecutor.VerifyAll();
    }
}