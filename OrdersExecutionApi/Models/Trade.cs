namespace OrdersExecutionApi.Models;

internal sealed record Trade(
    Way Way, 
    string Instrument, 
    decimal ExecutedQuantity, 
    decimal ExecutedPrice,
    DateTime ExecutionTime);