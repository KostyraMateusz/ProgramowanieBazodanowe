using BLL.DTOModels.GroupDTOs;
using BLL.DTOModels.ProductDTOs;
using BLL.ServiceInterfaces;
using DAL.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Model;

namespace BLL_EF
{
    public class ProductService : IProductService
    {
        private readonly WebStoreContext _context;

        public ProductService(WebStoreContext context)
        {
            _context = context;
        }

        // Pobranie pełnej nazwy grupy dla produktu
        private async Task<string> GetFullGroupNameAsync(int productId)
        {
            var product = await _context.Products
                .Include(p => p.ProductGroup)
                .ThenInclude(pg => pg.ParentGroup)
                .FirstOrDefaultAsync(p => p.ID == productId);

            if (product == null || product.ProductGroup == null)
            {
                throw new ArgumentException("Product not found or doesn't have a group.");
            }

            var groupNames = new List<string>();
            var currentGroup = product.ProductGroup;

            while (currentGroup != null)
            {
                groupNames.Insert(0, currentGroup.Name);
                currentGroup = await _context.ProductGroups
                    .Include(pg => pg.ParentGroup)
                    .FirstOrDefaultAsync(pg => pg.ID == currentGroup.ParentID);
            }

            return string.Join(" / ", groupNames);
        }


        // Pobranie produktów na podstawie filtrów
        public async Task<IEnumerable<ProductResponseDTO>> GetProducts(string? nameFilter, string? groupNameFilter, int? groupIdFilter, string? sortBy, bool sortOrder, bool includeInactive)
        {
            var productsQuery = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(nameFilter))
                productsQuery = productsQuery.Where(p => p.Name.Contains(nameFilter));

            if (groupIdFilter.HasValue)
                productsQuery = productsQuery.Where(p => p.GroupID == groupIdFilter.Value);

            if (!includeInactive)
                productsQuery = productsQuery.Where(p => p.IsActive);

            // Sortowanie wyników
            if (!string.IsNullOrEmpty(sortBy))
            {
                if (sortBy.ToLower() == "name")
                {
                    productsQuery = sortOrder ? productsQuery.OrderBy(p => p.Name) : productsQuery.OrderByDescending(p => p.Name);
                }
                else if (sortBy.ToLower() == "price")
                {
                    productsQuery = sortOrder ? productsQuery.OrderBy(p => p.Price) : productsQuery.OrderByDescending(p => p.Price);
                }
            }

            var products = await productsQuery.ToListAsync();

            // Tworzenie odpowiedzi z pełną nazwą grupy
            var productResponses = new List<ProductResponseDTO>();

            foreach (var product in products)
            {
                string fullGroupName = product.ProductGroup != null
                    ? await GetFullGroupNameAsync(product.ProductGroup.ID)
                    : "No group";

                productResponses.Add(new ProductResponseDTO
                {
                    ProductID = product.ID,
                    Name = product.Name,
                    Price = product.Price,
                    GroupName = fullGroupName
                });
            }

            return productResponses;
        }

        // Dodanie nowego produktu
        public async Task AddProduct(ProductRequestDTO productRequestDTO)
        {
            var product = new Product
            {
                Name = productRequestDTO.Name,
                Price = productRequestDTO.Price,
                GroupID = productRequestDTO.GroupID,
                IsActive = true
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        // Zmiana statusu produktu (np. aktywacja/dezaktywacja)
        public async Task ChangeProductStatus(int productId)
        {
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
                throw new Exception("Product not found");

            product.IsActive = !product.IsActive;
            await _context.SaveChangesAsync();
        }

        // Usunięcie produktu
        public async Task DeleteProduct(int productId)
        {
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
                throw new Exception("Product not found");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        // Pobranie grup produktów
        public async Task<IEnumerable<GroupResponseDTO>> GetGroups(int? parentId, string? sortBy, bool sortOrder)
        {
            var groupsQuery = _context.ProductGroups.AsQueryable();

            if (parentId.HasValue)
                groupsQuery = groupsQuery.Where(g => g.ParentID == parentId.Value);

            // Sortowanie
            if (!string.IsNullOrEmpty(sortBy))
            {
                if (sortBy.ToLower() == "name")
                {
                    groupsQuery = sortOrder ? groupsQuery.OrderBy(g => g.Name) : groupsQuery.OrderByDescending(g => g.Name);
                }
            }

            var groups = await groupsQuery.ToListAsync();

            return groups.Select(g => new GroupResponseDTO
            {
                Id = g.ID,
                Name = g.Name,
                HasChildren = g.Products.Any()
            });
        }

        // Dodanie nowej grupy produktów
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
