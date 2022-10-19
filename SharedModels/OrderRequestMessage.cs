using System;
using System.Collections.Generic;

namespace SharedModels
{
    public class OrderRequestMessage
    {
        public int? CustomerId { get; set; }
        public int OrderId { get; set; }
        public IList<OrderLine> OrderLines { get; set; }
    }
}

