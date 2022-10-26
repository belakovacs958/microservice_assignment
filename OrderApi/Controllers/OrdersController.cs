using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Data;
using OrderApi.Infrastructure;
using SharedModels;
using System.Net.Http.Json;

namespace OrderApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        IOrderRepository repository;
        IMessagePublisher messagePublisher;
        HttpClient httpClient = new HttpClient();

        public OrdersController(IRepository<Order> repos,
            IMessagePublisher publisher)
        {
            repository = repos as IOrderRepository;
            messagePublisher = publisher;
        }

        // GET orders
        [HttpGet]
        public IEnumerable<Order> Get()
        {
            return repository.GetAll();
        }

        // GET orders/5
        [HttpGet("{id}", Name = "GetOrder")]
        public IActionResult Get(int id)
        {
            var item = repository.Get(id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        // GET orders/1 by user
        [HttpGet("ordersOfCustomer/{uid}", Name = "GetOrdersOfCustomer")]
        public IEnumerable<Order> GetOrdersOfCustomer(int uid)
        {
            return repository.GetByCustomer(uid);
        }

        // POST orders
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody]Order order)
        {
            // TODO implement credit check on use

            bool areOrderedProductsAvailable = false;
            bool doesCustomerExistvar = false;
            if (order == null)
            {
                return BadRequest();
            }


            foreach (var orderline in order.OrderLines)
            {

               areOrderedProductsAvailable =  await isProductAvailable(orderline.ProductId, orderline.Quantity); 
                if (areOrderedProductsAvailable == false)
                {
                    break;
                }

            }

            doesCustomerExistvar = await doesCustomerExist((int)order.customerId);


            if (doesCustomerExistvar && areOrderedProductsAvailable)
            {
          
                try
                {
                    // Create a tentative order.
                    order.Status = Order.OrderStatus.tentative;
                    var newOrder = repository.Add(order);

                    // Publish OrderStatusChangedMessage. 
                    messagePublisher.PublishOrderCreatedMessage(
                        newOrder.customerId, newOrder.Id, newOrder.OrderLines);
                    
                    
                    // Wait until order status is "completed"
                    bool completed = false;
                    while (!completed)
                    {
                        var tentativeOrder = repository.Get(newOrder.Id);
                        if (tentativeOrder.Status == Order.OrderStatus.completed)
                            completed = true;
                        Thread.Sleep(100);
                    }

                    return CreatedAtRoute("GetOrder", new { id = newOrder.Id }, newOrder);
                }
                catch
                {
                    return StatusCode(500, "An error happened. Try again.");
                }
                Console.WriteLine("everything is fine");
            }
            else
            {
      
                Console.WriteLine("not everything is fine");

                messagePublisher.PublishOrderRejectedMessage((int)order.customerId,order.Id);
                return StatusCode(500, "We dont have enough of a product or user does not exist");
            }


           
        }


        // PUT orders/5/cancel
        // This action method cancels an order and publishes an OrderStatusChangedMessage
        // with topic set to "cancelled".
        [HttpPut("{id}/cancel")]
        public IActionResult Cancel(int id)
        {
           

            try
            {
                var order = repository.Get(id);
                if (order.Status == Order.OrderStatus.completed)
                    // Create a tentative order.
                    order.Status = Order.OrderStatus.cancelled;
                repository.Edit(order);

                // Publish OrderStatusChangedMessage. 
                messagePublisher.PublishOrderCancelledMessage(
                    (int)order.customerId, order.Id, order.OrderLines );


                return StatusCode(201);
            }
            catch
            {
                return StatusCode(500, "An error happened. Try again.");
            }

        }

        // PUT orders/5/ship
        // This action method ships an order and publishes an OrderStatusChangedMessage.
        // with topic set to "shipped".
        [HttpPut("{id}/ship")]
        public IActionResult Put([FromBody] Order order)
        {

            // TODO implement ship same as cancel so it doesnt need an order input, only id
            if (order == null)
            {
                return BadRequest();
            }
            try
            {
                // Create a tentative order.
                order.Status = Order.OrderStatus.shipped;
                repository.Edit(order);

                // Publish OrderStatusChangedMessage. 
                messagePublisher.PublishOrderShippedMessage(
                    (int)order.customerId, order.Id, order.OrderLines);


                return StatusCode(201);
            }
            catch
            {
                return StatusCode(500, "An error happened. Try again.");
            }

            // Add code to implement this method.
        }

        // PUT orders/5/pay
        // This action method marks an order as paid and publishes a CreditStandingChangedMessage
        // (which have not yet been implemented), if the credit standing changes.
        [HttpPut("{id}/pay")]
        public IActionResult Pay(int id)
        {
            throw new NotImplementedException();

            // Add code to implement this method.
        }


        private async Task<bool> isProductAvailable(int productId, int productQuantity)
        {

            try
            {
                ProductDto response  = await httpClient.GetFromJsonAsync<ProductDto>("http://192.168.5.110:8080/products/" + productId);
                Console.WriteLine("this is the number of items in stock" + response.ItemsInStock.ToString());
                if (response != null && response.ItemsInStock > productQuantity)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
          
        }

        public async Task<bool> doesCustomerExist(int customerId)
        {

            try
            {
                UserDto response = await httpClient.GetFromJsonAsync<UserDto>("http://192.168.5.110:7080/user/" + customerId);
       
                if (response != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }

    }
}
