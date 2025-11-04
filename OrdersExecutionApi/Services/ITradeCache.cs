using System.Diagnostics.CodeAnalysis;
using OrdersExecutionApi.Models;

namespace OrdersExecutionApi.Services;

internal interface ITradeCache
{
    bool TryGetTrade(Order order, [NotNullWhen(true)] out Trade? trade);
    void Add(Order order, Trade trade);
}