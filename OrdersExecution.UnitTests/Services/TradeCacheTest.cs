using System.Collections.Concurrent;
using OrdersExecutionApi.Models;
using OrdersExecutionApi.Services;

namespace OrdersExecution.UnitTests.Services;

[TestFixture]
internal sealed class TradeCacheTest
{
    [Test] public void TestTryGetTradeWhenCacheIsEmpty()
    {
        var order = new Order(Way.Buy, string.Empty, 10, 10, DateTime.Now);
        var tradeCache = new TradeCache();
        Assert.That(tradeCache.TryGetTrade(order, out var trade), Is.False);
        Assert.That(trade, Is.Null);
    }

    [Test] public void TestTryGetTradeWhenOrderIsAlreadyInCache()
    {
        var order = new Order(Way.Buy, string.Empty, 10, 10, DateTime.Now);
        var trade = new Trade(Way.Buy, string.Empty, 10, 10, DateTime.Now.AddMinutes(1));
        var tradeCache = new TradeCache(new ConcurrentDictionary<Order, Trade>
        {
            [order] = trade
        });
        Assert.That(tradeCache.TryGetTrade(order, out var cachedTrade), Is.True);
        Assert.That(cachedTrade, Is.Not.Null);
    }

    [Test] public void TestAddToCacheWhenEmpty()
    {
        var order = new Order(Way.Buy, string.Empty, 10, 10, DateTime.Now);
        var trade = new Trade(Way.Buy, string.Empty, 10, 10, DateTime.Now.AddMinutes(1));
        var concurrentDict = new ConcurrentDictionary<Order, Trade>();
        var tradeCache = new TradeCache(concurrentDict);
        tradeCache.Add(order, trade);
        Assert.That(concurrentDict, Has.Count.EqualTo(1));
        Assert.That(concurrentDict[order], Is.EqualTo(trade));
    }

    [Test] public void TestAddToCacheWhenSameKeyExist()
    {
        var order = new Order(Way.Buy, string.Empty, 10, 10, DateTime.Now);
        var trade = new Trade(Way.Buy, string.Empty, 10, 10, DateTime.Now.AddMinutes(1));
        var concurrentDict = new ConcurrentDictionary<Order, Trade>
        {
            [order] = trade
        };
        var tradeCache = new TradeCache(concurrentDict);
        tradeCache.Add(order, trade);
        Assert.That(concurrentDict, Has.Count.EqualTo(1));
        Assert.That(concurrentDict[order], Is.EqualTo(trade));
    }
}