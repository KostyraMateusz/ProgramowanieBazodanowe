using BLL.DTOModels.BasketDTOs;
using BLL.DTOModels.OrderDTOs;
using BLL.ServiceInterfaces;
using BLL_MongoDb.ModelMongo;
using MongoDB.Driver;

namespace BLL_MongoDb.ServiceMongo
{
    public class BasketServiceMongo : IBasketService
    {
        private readonly SequenceService _sequenceService;
        private readonly IMongoCollection<BasketItemMongo> _basketCollection;
        private readonly IMongoCollection<ProductMongo> _productCollection;
        private readonly IMongoCollection<OrderMongo> _orderCollection;

        public BasketServiceMongo(IMongoDatabase database)
        {
            _sequenceService = new SequenceService(database);
            _basketCollection = database.GetCollection<BasketItemMongo>("basket");
            _productCollection = database.GetCollection<ProductMongo>("products");
            _orderCollection = database.GetCollection<OrderMongo>("orders");
        }

        public async Task AddProductToBasket(BasketRequestDTO basketRequest)
        {
            var existingItem = await _basketCollection
                .Find(x => x.UserId == basketRequest.UserID && x.ProductId == basketRequest.ProductID)
                .FirstOrDefaultAsync();

            if (existingItem != null)
            {
                existingItem.Amount += basketRequest.Amount;
                await _basketCollection.ReplaceOneAsync(x => x.Id == existingItem.Id, existingItem);
            }
            else
            {
                var newItem = new BasketItemMongo
                {
                    Id = _sequenceService.GetNextSequence("basket"),
                    ProductId = basketRequest.ProductID,
                    UserId = basketRequest.UserID,
                    Amount = basketRequest.Amount
                };

                await _basketCollection.InsertOneAsync(newItem);
            }
        }

        public async Task<OrderResponseDTO> CreateOrder(int userId)
        {
            var basketItems = await _basketCollection.Find(x => x.UserId == userId).ToListAsync();

            if (!basketItems.Any())
                return new OrderResponseDTO();

            var productList = await _productCollection.Find(_ => true).ToListAsync();
            var productMap = productList.ToDictionary(p => p.Id);

            var orderItems = basketItems.Select(item =>
            {
                var product = productMap[item.ProductId];
                return new OrderItemMongo
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Amount = item.Amount
                };
            }).ToList();

            var newOrder = new OrderMongo
            {
                Id = _sequenceService.GetNextSequence("orders"),
                UserId = userId,
                OrderPositions = orderItems,
                IsPaid = false,
                Date = DateTime.UtcNow
            };

            await _orderCollection.InsertOneAsync(newOrder);
            await _basketCollection.DeleteManyAsync(x => x.UserId == userId);

            return new OrderResponseDTO
            {
                OrderID = newOrder.Id,
                OrderDate = newOrder.Date,
                TotalPrice = orderItems.Sum(i => i.Price * i.Amount),
                IsPaid = false
            };
        }

        public async Task RemoveFromBasket(int userId, int productId)
        {
            await _basketCollection.DeleteOneAsync(x => x.UserId == userId && x.ProductId == productId);
        }

        public async Task UpdateBasketItem(int userId, int productId, int amount)
        {
            var item = await _basketCollection
                .Find(x => x.UserId == userId && x.ProductId == productId)
                .FirstOrDefaultAsync();

            if (item != null)
            {
                item.Amount = amount;
                await _basketCollection.ReplaceOneAsync(x => x.Id == item.Id, item);
            }
        }
    }
}
