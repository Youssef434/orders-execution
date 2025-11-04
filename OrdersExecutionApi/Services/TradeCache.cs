using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using OrdersExecutionApi.Models;

namespace OrdersExecutionApi.Services;

internal sealed class TradeCache : ITradeCache
{
    private readonly ConcurrentDictionary<Order, Trade> _cache;
    public TradeCache() : this(new ConcurrentDictionary<Order, Trade>())
    {
    }
    public TradeCache(ConcurrentDictionary<Order, Trade> cache)
    {
        _cache = cache;
    }
    public bool TryGetTrade(Order order, [NotNullWhen(true)] out Trade? trade)
    {
        if (_cache.TryGetValue(order, out var cachedTrade))
        {
            trade = cachedTrade;
            return true;
        }
        trade = null;
        return false;
    }
    public void Add(Order order, Trade trade)
    {
        _cache.TryAdd(order, trade);
    }
}