using System;
using SharedModels;

namespace UserApi.Infrastructure
{   
        public interface IMessagePublisher
        {
            void PublishOrderRequestMessage(int? customerId, int orderId,
                IList<OrderLine> orderLines);
        }
}

