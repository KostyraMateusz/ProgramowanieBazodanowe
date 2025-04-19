using BLL.DTOModels.GroupDTOs;
using BLL.DTOModels.ProductDTOs;
using BLL.ServiceInterfaces;
using BLL_MongoDb.ModelMongo;
using MongoDB.Driver;

namespace BLL_MongoDb.ServiceMongo
{
    public class ProductServiceMongo : IProductService
    {
        private readonly SequenceService _sequenceService;
        private readonly IMongoCollection<ProductMongo> _products;
        private readonly IMongoCollection<ProductGroupMongo> _groups;

        public ProductServiceMongo(IMongoDatabase database)
        {
            _sequenceService = new SequenceService(database);
            _products = database.GetCollection<ProductMongo>("products");
            _groups = database.GetCollection<ProductGroupMongo>("groups");
        }

        public async Task AddGroup(GroupRequestDTO dto)
        {
            var group = new ProductGroupMongo
            {
                Id = _sequenceService.GetNextSequence("groups"),
                Name = dto.Name,
                ParentId = dto.ParentId
            };

            await _groups.InsertOneAsync(group);
        }

        public async Task AddProduct(ProductRequestDTO dto)
        {
            var product = new ProductMongo
            {
                Id = _sequenceService.GetNextSequence("products"),
                Name = dto.Name,
                Price = dto.Price,
                GroupId = dto.GroupID,
                IsActive = true
            };

            await _products.InsertOneAsync(product);
        }

        public async Task ChangeProductStatus(int productId)
        {
            var product = await _products.Find(p => p.Id == productId).FirstOrDefaultAsync();
            if (product != null)
            {
                product.IsActive = !product.IsActive;
                await _products.ReplaceOneAsync(p => p.Id == productId, product);
            }
        }

        public async Task DeleteProduct(int productId)
        {
            await _products.DeleteOneAsync(p => p.Id == productId);
        }

        public async Task<IEnumerable<GroupResponseDTO>> GetGroups(int? parentId, string? sortBy, bool sortOrder)
        {
            var allGroups = await _groups.Find(_ => true).ToListAsync();
            var filtered = parentId.HasValue
                ? allGroups.Where(g => g.ParentId == parentId)
                : allGroups.Where(g => g.ParentId == null);

            var result = filtered.Select(g =>
            {
                var hasChildren = allGroups.Any(child => child.ParentId == g.Id);
                return new GroupResponseDTO
                {
                    Id = g.Id,
                    Name = g.Name,
                    ParentId = g.ParentId,
                    HasChildren = hasChildren
                };
            });

            return sortBy?.ToLower() == "name"
                ? (sortOrder ? result.OrderBy(g => g.Name) : result.OrderByDescending(g => g.Name))
                : result;
        }

        public async Task<IEnumerable<ProductResponseDTO>> GetProducts(string? nameFilter, string? groupNameFilter, int? groupIdFilter, string? sortBy, bool sortOrder, bool includeInactive)
        {
            var filterBuilder = Builders<ProductMongo>.Filter;
            var filter = includeInactive ? FilterDefinition<ProductMongo>.Empty : filterBuilder.Eq(p => p.IsActive, true);

            if (!string.IsNullOrEmpty(nameFilter))
                filter &= filterBuilder.Regex(p => p.Name, new MongoDB.Bson.BsonRegularExpression(nameFilter, "i"));

            var allGroups = await _groups.Find(_ => true).ToListAsync();
            var groupMap = allGroups.ToDictionary(g => g.Id);

            if (!string.IsNullOrEmpty(groupNameFilter))
            {
                var matchingGroupIds = allGroups
                    .Where(g =>
                    {
                        // Budowanie ścieżki grupy bez wywoływania osobnej metody
                        var hierarchy = new List<string> { g.Name };
                        var current = g;
                        while (current.ParentId.HasValue && groupMap.TryGetValue(current.ParentId.Value, out var parent))
                        {
                            hierarchy.Insert(0, parent.Name);
                            current = parent;
                        }
                        var groupPath = string.Join(" / ", hierarchy);
                        return groupPath.Contains(groupNameFilter, StringComparison.OrdinalIgnoreCase);
                    })
                    .Select(g => g.Id)
                    .ToHashSet();

                filter &= filterBuilder.In(p => p.GroupId, matchingGroupIds);
            }

            if (groupIdFilter.HasValue)
            {
                var descendantIds = new HashSet<int> { groupIdFilter.Value };
                var queue = new Queue<int>();
                queue.Enqueue(groupIdFilter.Value);

                while (queue.Count > 0)
                {
                    var current = queue.Dequeue();
                    foreach (var child in allGroups.Where(g => g.ParentId == current))
                    {
                        if (descendantIds.Add(child.Id))
                            queue.Enqueue(child.Id);
                    }
                }

                filter &= filterBuilder.In(p => p.GroupId, descendantIds);
            }

            var products = await _products.Find(filter).ToListAsync();

            var result = products.Select(p =>
            {
                var groupName = groupMap.TryGetValue(p.GroupId, out var group)
                    ? BuildGroupPath(group, groupMap)
                    : string.Empty;

                return new ProductResponseDTO
                {
                    ProductID = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    GroupName = groupName
                };
            });

            return sortBy switch
            {
                "name" => sortOrder ? result.OrderBy(p => p.Name) : result.OrderByDescending(p => p.Name),
                "price" => sortOrder ? result.OrderBy(p => p.Price) : result.OrderByDescending(p => p.Price),
                "group" => sortOrder ? result.OrderBy(p => p.GroupName) : result.OrderByDescending(p => p.GroupName),
                _ => result
            };
        }

        private static string BuildGroupPath(ProductGroupMongo group, Dictionary<int, ProductGroupMongo> allGroups)
        {
            var hierarchy = new List<string> { group.Name };
            var current = group;

            while (current.ParentId.HasValue && allGroups.TryGetValue(current.ParentId.Value, out var parent))
            {
                hierarchy.Insert(0, parent.Name);
                current = parent;
            }

            return string.Join(" / ", hierarchy);
        }
    }
}
