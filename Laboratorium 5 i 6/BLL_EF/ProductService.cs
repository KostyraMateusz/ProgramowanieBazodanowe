using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BLL.DTOModels.GroupDTOs;
using BLL.DTOModels.ProductDTOs;
using BLL.ServiceInterfaces;
using DAL;
using DAL.Model;
using Microsoft.EntityFrameworkCore;
using Model;

namespace BLL_EF.Services
{
    public class ProductService : IProductService
    {
        private readonly WebStoreContext _context;

        public ProductService(WebStoreContext context)
        {
            _context = context;
        }

        
        public async Task<IEnumerable<ProductResponseDTO>> GetProducts(string? nameFilter, string? groupNameFilter, int? groupIdFilter, string? sortBy, bool sortOrder, bool includeInactive)
        {
            var products = await _context.Products
                .Include(p => p.Group)
                .Where(p => includeInactive || p.IsActive)
                .ToListAsync();

            var productResponseDTOs = new List<ProductResponseDTO>();

            foreach (var product in products)
            {
                var groupName = product.Group != null ? await GetFullGroupHierarchyAsync(product.GroupID) : string.Empty;

                if (!string.IsNullOrEmpty(nameFilter) && !product.Name.Contains(nameFilter))
                    continue;

                if (!string.IsNullOrEmpty(groupNameFilter) && !groupName.Contains(groupNameFilter))
                    continue;

                if (groupIdFilter.HasValue && !DoesGroupMatchHierarchy(product.GroupID, groupIdFilter.Value))
                    continue;

                productResponseDTOs.Add(new ProductResponseDTO
                {
                    ProductID = product.ID,
                    Name = product.Name,
                    Price = product.Price,
                    GroupName = groupName
                });
            }

            productResponseDTOs = sortBy switch
            {
                "name" => sortOrder ? productResponseDTOs.OrderBy(p => p.Name).ToList()
                                    : productResponseDTOs.OrderByDescending(p => p.Name).ToList(),
                "price" => sortOrder ? productResponseDTOs.OrderBy(p => p.Price).ToList()
                                     : productResponseDTOs.OrderByDescending(p => p.Price).ToList(),
                "group" => sortOrder ? productResponseDTOs.OrderBy(p => p.GroupName).ToList()
                                     : productResponseDTOs.OrderByDescending(p => p.GroupName).ToList(),
                _ => productResponseDTOs
            };

            return productResponseDTOs;
        }

        private async Task<string> GetFullGroupHierarchyAsync(int? groupId)
        {
            if (!groupId.HasValue)
                return string.Empty;

            List<string> hierarchy = new List<string>();

            var currentGroup = await _context.ProductGroups
                .Include(g => g.Parent)
                .FirstOrDefaultAsync(g => g.ID == groupId.Value);

            while (currentGroup != null)
            {
                hierarchy.Insert(0, currentGroup.Name);
                if (currentGroup.ParentID.HasValue)
                    currentGroup = await _context.ProductGroups
                        .Include(g => g.Parent)
                        .FirstOrDefaultAsync(g => g.ID == currentGroup.ParentID.Value);
                else
                    currentGroup = null;
            }

            return string.Join(" / ", hierarchy);
        }

        private bool DoesGroupMatchHierarchy(int? productGroupId, int targetGroupId)
        {
            if (!productGroupId.HasValue)
                return false;

            var currentGroup = _context.ProductGroups
                .Include(g => g.Parent)
                .FirstOrDefault(g => g.ID == productGroupId.Value);

            while (currentGroup != null)
            {
                if (currentGroup.ID == targetGroupId)
                    return true;

                currentGroup = currentGroup.Parent;
            }

            return false;
        }


        public async Task AddProduct(ProductRequestDTO productDto)
        {
            var product = new Product
            {
                Name = productDto.Name,
                Price = productDto.Price,
                GroupID = productDto.GroupID,
                IsActive = true
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task ChangeProductStatus(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                product.IsActive = !product.IsActive;
                await _context.SaveChangesAsync();
            }
        }
        
        public async Task DeleteProduct(int productId)
        {
            var product = await _context.Products
                .Include(p => p.OrderPositions)
                .FirstOrDefaultAsync(p => p.ID == productId);

            if (product == null)
                throw new InvalidOperationException("Product not found.");

            if (product.OrderPositions.Any())
                throw new InvalidOperationException("Cannot delete a product that is linked to order items.");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
        
        public async Task<IEnumerable<GroupResponseDTO>> GetGroups(int? parentId, string? sortBy, bool sortOrder)
        {
            var query = _context.ProductGroups.AsQueryable();

            if (parentId.HasValue)
                query = query.Where(g => g.ParentID == parentId.Value);
            else
                query = query.Where(g => g.ParentID == null);

            query = sortBy switch
            {
                "name" => sortOrder ? query.OrderBy(g => g.Name) : query.OrderByDescending(g => g.Name),
                _ => query
            };

            return await query.Select(g => new GroupResponseDTO
            {
                Id = g.ID,
                Name = g.Name,
                HasChildren = g.SubGroups.Any()
            }).ToListAsync();
        }

        public async Task AddGroup(GroupRequestDTO groupRequestDTO)
        {
            var group = new ProductGroup
            {
                Name = groupRequestDTO.Name,
                ParentID = groupRequestDTO.ParentId
            };

            _context.ProductGroups.Add(group);
            await _context.SaveChangesAsync();
        }
    }
}