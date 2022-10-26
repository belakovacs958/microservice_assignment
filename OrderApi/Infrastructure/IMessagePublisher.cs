﻿using System.Collections.Generic;
using SharedModels;

namespace OrderApi.Infrastructure
{
    public interface IMessagePublisher
    {
        void PublishOrderCreatedMessage(int? customerId, int orderId,
            IList<OrderLine> orderLines);
        void PublishOrderRejectedMessage(int customerId, int orderId);
        // TODO implement all of these methods below
        /*
        void PublishOrderPaidMessage(int customerId, int orderId);
        void PublishOrderShippedMessage(int customerId, int orderId);
        void PublishOrderCancelledMessage(int customerId, int orderId);
        void PublishOrderAcceptedMessage(int customerId, int orderId);
        */
    }
}
