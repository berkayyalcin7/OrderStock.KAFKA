using Order.API.DTOs;
using Shared.Event;
using Shared.Event.Events;

namespace Order.API.Services
{
    public class OrderService(IBus bus)
    {
        public async Task<bool> Create(OrderCreateRequestDto request)
        {
            // Save to db işlemi yaptık .
            var orderCode = Guid.NewGuid().ToString();
            var orderCreatedEvent = new OrderCreatedEvent(orderCode, request.UserId, request.TotalPrice);

           return await bus.Publish(orderCode, orderCreatedEvent, BusConsts.OrderCreatedEventTopicName);
        }
    }
}
