namespace OrdersExecution
{
    public class OrdersExecutionExercise
    {
        #region Dépendance Externe - NE PAS MODIFIER
        /// <summary>
        /// Énumération de la dépendance externe
        /// </summary>
        public enum Way
        {
            Buy = 1,
            Sell = 2
        }
        /// <summary>
        /// Modèle d'ordre de la dépendance externe
        /// </summary>
        public class Order
        { 
            public Way Way { get; set; }
            public string Instrument { get; set; }
            public decimal Quantity { get; set; }
            public decimal LimitPrice { get; set; }
            public DateTime OrderDate { get; set; }

        }
        /// <summary>
        /// Modèle de trade de la dépendance externe
        /// </summary>
        public class Trade
        { 
            public Way Way { get; set; }
            public string Instrument { get; set; }
            public decimal ExecutedQuantity { get; set; }
            public decimal ExecutedPrice { get; set; }
            public DateTime ExecutionTime { get; set; }
        }
        /// <summary>
        /// Interface de la dépendance externe (NuGet tiers)
        /// </summary>
        public interface IOrderExecutor
        { 
            Task<Trade> ExecuteOrderAsync(Order order, CancellationToken cancellationToken = default);
        }
        /// <summary>
        /// Implémentation simulée de la dépendance - NE PAS MODIFIER
        /// </summary>
        public class OrderExecutor : IOrderExecutor
        { 
            private static readonly Random RandomInstance = new Random();
            public async Task<Trade> ExecuteOrderAsync(Order order, CancellationToken cancellationToken = default)
            { 
                if (order == null) throw new ArgumentNullException(nameof(order));
                // Simulation d'un délai d'exécution
                await Task.Delay(RandomInstance.Next(200, 1000), cancellationToken);
                return new Trade
                { 
                    Way = order.Way,
                    Instrument = order.Instrument,
                    ExecutedQuantity = order.Quantity,
                    ExecutedPrice = order.LimitPrice,
                    ExecutionTime = order.OrderDate.AddMinutes(1)
                };
            }
        }
        #endregion 
    }
}