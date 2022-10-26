using System.Collections.Generic;

namespace SharedModels
{
    public class OrderCancelledMessage
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public IList<OrderLine> OrderLines { get; set; }
    }
}
