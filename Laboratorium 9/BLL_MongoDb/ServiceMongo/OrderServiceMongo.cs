using BLL.DTOModels.OrderDTOs;
using BLL.ServiceInterfaces;
using BLL_MongoDb.ModelMongo;
using MongoDB.Driver;

namespace BLL_MongoDb.ServiceMongo
{
    public class OrderServiceMongo : IOrderService
    {
        private readonly IMongoCollection<OrderMongo> _orderCollection;

        public OrderServiceMongo(IMongoDatabase database)
        {
            _orderCollection = database.GetCollection<OrderMongo>("orders");
        }

        public async Task<IEnumerable<OrderResponseDTO>> GetOrder(int? orderIdFilter, bool? paidFilter, string? sortBy, bool sortOrder)
        {
            var filter = Builders<OrderMongo>.Filter.Empty;

            if (orderIdFilter.HasValue)
                filter &= Builders<OrderMongo>.Filter.Eq(x => x.Id, orderIdFilter.Value);

            if (paidFilter.HasValue)
                filter &= Builders<OrderMongo>.Filter.Eq(x => x.IsPaid, paidFilter.Value);

            var orders = await _orderCollection.Find(filter).ToListAsync();

            var mappedOrders = orders.Select(o => new OrderResponseDTO
            {
                OrderID = o.Id,
                OrderDate = o.Date,
                IsPaid = o.IsPaid,
                TotalPrice = o.OrderPositions.Sum(p => p.Price * p.Amount)
            });

            return sortBy?.ToLower() switch
            {
                "orderdate" => sortOrder ? mappedOrders.OrderBy(x => x.OrderDate) : mappedOrders.OrderByDescending(x => x.OrderDate),
                "totalprice" => sortOrder ? mappedOrders.OrderBy(x => x.TotalPrice) : mappedOrders.OrderByDescending(x => x.TotalPrice),
                "orderid" => sortOrder ? mappedOrders.OrderBy(x => x.OrderID) : mappedOrders.OrderByDescending(x => x.OrderID),
                _ => mappedOrders
            };
        }

        public async Task<OrderDetailsResponseDTO> GetOrderDetails(int orderId)
        {
            var order = await _orderCollection.Find(x => x.Id == orderId).FirstOrDefaultAsync();

            if (order == null)
                return new OrderDetailsResponseDTO { OrderPositions = new List<OrderItemDTO>() };

            return new OrderDetailsResponseDTO
            {
                OrderPositions = order.OrderPositions.Select(p => new OrderItemDTO
                {
                    ProductName = p.ProductName,
                    Price = p.Price,
                    Amount = p.Amount,
                    TotalValue = p.Price * p.Amount
                }).ToList()
            };
        }

        public async Task PayOrder(int orderId, double paymentAmount)
        {
            var order = await _orderCollection.Find(x => x.Id == orderId).FirstOrDefaultAsync();

            if (order == null) return;

            var totalOrderAmount = order.OrderPositions.Sum(p => p.Price * p.Amount);

            if (Math.Abs(totalOrderAmount - paymentAmount) < 0.01)
            {
                order.IsPaid = true;
                await _orderCollection.ReplaceOneAsync(x => x.Id == order.Id, order);
            }
            else
            {
                throw new Exception("Kwota płatności nie odpowiada całkowitej wartości zamówienia.");
            }
        }
    }
}
