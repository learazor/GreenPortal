using Entities.model.portal;
using GreenPortal.model;

namespace EfcRepositories;

public class OrderRepository
{
        private readonly GreenPortalContext _context;

        public OrderRepository(GreenPortalContext context)
        {
            _context = context;
        }
        
        public async Task<InstallationOrder?> GetOrderByIdAsync(Guid orderId)
        {
            return await _context.InstallationOrders.FindAsync(orderId);
        }
        
        public async Task AddOrder(InstallationOrder order)
        {
            await _context.InstallationOrders.AddAsync(order);
            await _context.SaveChangesAsync();
        }
        
        public async Task UpdateOrderStatus(Guid orderId, OrderStatus newStatus)
        {
            var order = await _context.InstallationOrders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = newStatus;
                await _context.SaveChangesAsync();
            }
        }
        
}