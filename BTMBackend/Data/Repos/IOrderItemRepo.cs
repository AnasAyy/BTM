using BTMBackend.Dtos.OrderItemDto;
using BTMBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BTMBackend.Data.Repos
{
    public interface IOrderItemRepo
    {
        public Task<bool> CheckIfDataExistByOrderId(int orderId);
        public Task<bool> CheckOrderItems(int orderId);
        public Task<List<OrderItem>> GetByOrderId(int orderId);
        public Task<bool> Add(OrderItemsRequestDto requestDto);
        public Task<bool> Delete(int orderId);

        //public Task<bool> Update(UpdateOrderItemsRequestDto requestDto);
    }

    public class OrderItemRepo(DataContext context) : IOrderItemRepo
    {
        private readonly DataContext _Context = context;

        public async Task<bool> Add(OrderItemsRequestDto requestDto)
        {
            var orderItems = new List<OrderItem>();

            for (int i = 0; i < requestDto.ItemId.Length; i++)
            {
                orderItems.Add(new OrderItem()
                {
                    OrderId = requestDto.OrderId,
                    ItemId = requestDto.ItemId[i],
                    ItemType = requestDto.ItemType[i],
                    ServiceType = requestDto.ServiceType[i],
                    CustomerPartId = requestDto.CustomerPartId[i],
                });
            }

            await _Context.OrderItems.AddRangeAsync(orderItems);
            return await SaveChanges();
        }

        public Task<bool> CheckIfDataExistByOrderId(int orderId)
        {
            return _Context.OrderItems.AnyAsync(x => x.OrderId == orderId);
        }

        public async Task<bool> CheckOrderItems(int orderId)
        {
            return await _Context.OrderStatusHistories.AnyAsync(x => x.OrderId == orderId);
        }

        public async Task<bool> Delete(int orderId)
        {
            var getOrderItems = await _Context.OrderItems.Where(x => x.OrderId == orderId).ToListAsync();
            _Context.OrderItems.RemoveRange(getOrderItems);
            return await SaveChanges();
        }

        public async Task<List<OrderItem>> GetByOrderId(int orderId)
        {
            return await _Context.OrderItems.Where(x => x.OrderId == orderId).ToListAsync();
        }

        private async Task<bool> SaveChanges()
        {
            try
            {
                return await _Context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _Context.ChangeTracker.Clear();
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
        //public Task<bool> Update(UpdateOrderItemsRequestDto requestDto)
        //{
        //    var checkOrderItems = 
        //}
    }
}
