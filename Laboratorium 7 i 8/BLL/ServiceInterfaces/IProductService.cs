using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.DTOModels.GroupDTOs;
using BLL.DTOModels.ProductDTOs;

namespace BLL.ServiceInterfaces
{
    public interface IProductService
    {
        public Task<IEnumerable<ProductResponseDTO>> GetProducts(string? nameFilter, string? groupNameFilter, int? groupIdFilter, string? sortBy, bool sortOrder, bool includeInactive);

        public Task AddProduct(ProductRequestDTO productRequestDTO);
        public Task ChangeProductStatus(int productId);
        public Task DeleteProduct(int productId);

        public Task<IEnumerable<GroupResponseDTO>> GetGroups(int? parentId, string? sortBy, bool sortOrder);
        public Task AddGroup(GroupRequestDTO groupRequestDTO);
    }
}
