using OrdersExecution;
using OrdersExecutionApi.Models;

namespace OrdersExecutionApi.Converters;

internal static class ConvertersExtensions
{
    public static OrdersExecutionExercise.Order Convert(this Order order)
    {
        return new OrdersExecutionExercise.Order
        {
            Way = (OrdersExecutionExercise.Way)(int)order.Way,
            Instrument = order.Instrument,
            Quantity = order.Quantity,
            LimitPrice = order.LimitPrice,
            OrderDate = order.OrderDate
        };
    }

    public static Trade Convert(this OrdersExecutionExercise.Trade trade)
    {
        return new Trade(
            (Way)(int)trade.Way,
            trade.Instrument,
            trade.ExecutedQuantity,
            trade.ExecutedPrice,
            trade.ExecutionTime
        );
    }
}