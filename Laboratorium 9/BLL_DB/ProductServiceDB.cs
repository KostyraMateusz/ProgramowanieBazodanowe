using BLL.DTOModels.GroupDTOs;
using BLL.DTOModels.ProductDTOs;
using BLL.ServiceInterfaces;
using DAL.Model;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_DB
{
    public class ProductServiceDB : IProductService
    {
        private readonly WebStoreContext _context;
        private readonly string _connectionString;

        public ProductServiceDB(WebStoreContext context)
        {
            _context = context;
            _connectionString = ("Data Source=(localdb)\\\\MSSQLLocalDB;Initial Catalog=DbSklep;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False\"");
        }

        public async Task<IEnumerable<ProductResponseDTO>> GetProducts(string? nameFilter, string? groupNameFilter, int? groupIdFilter, string? sortBy, bool sortOrder, bool includeInactive)
        {
            using var connection = new SqlConnection(_connectionString);

            string sql = @"
                    WITH GroupHierarchy AS (
                        SELECT 
                            ID,
                            Name,
                            ParentID,
                            CAST(Name AS NVARCHAR(MAX)) AS FullPath
                        FROM ProductGroups
                        WHERE ParentID IS NULL

                        UNION ALL

                        SELECT 
                            pg.ID,
                            pg.Name,
                            pg.ParentID,
                            CAST(gh.FullPath + ' / ' + pg.Name AS NVARCHAR(MAX)) AS FullPath
                        FROM ProductGroups pg
                        INNER JOIN GroupHierarchy gh ON pg.ParentID = gh.ID
                    )

                    SELECT 
                        p.ID AS ProductID,
                        p.Name,
                        p.Price,
                        ISNULL(gh.FullPath, '') AS GroupName
                    FROM Products p
                    LEFT JOIN GroupHierarchy gh ON p.GroupID = gh.ID
                    WHERE
                        (@IncludeInactive = 1 OR p.IsActive = 1)
                        AND (@NameFilter IS NULL OR p.Name LIKE '%' + @NameFilter + '%')
                        AND (@GroupNameFilter IS NULL OR gh.FullPath LIKE '%' + @GroupNameFilter + '%')
                        AND (
                            @GroupIdFilter IS NULL
                            OR EXISTS (
                                SELECT 1
                                FROM GroupHierarchy gh2
                                WHERE gh2.ID = p.GroupID
                                  AND (
                                    gh2.ID = @GroupIdFilter
                                    OR gh2.FullPath LIKE (
                                        SELECT FullPath + '%' FROM GroupHierarchy WHERE ID = @GroupIdFilter
                                    )
                                  )
                            )
                        )
                    ORDER BY
                        CASE WHEN @SortBy = 'name' AND @SortOrder = 1 THEN p.Name END ASC,
                        CASE WHEN @SortBy = 'name' AND @SortOrder = 0 THEN p.Name END DESC,
                        CASE WHEN @SortBy = 'price' AND @SortOrder = 1 THEN p.Price END ASC,
                        CASE WHEN @SortBy = 'price' AND @SortOrder = 0 THEN p.Price END DESC,
                        CASE WHEN @SortBy = 'group' AND @SortOrder = 1 THEN gh.FullPath END ASC,
                        CASE WHEN @SortBy = 'group' AND @SortOrder = 0 THEN gh.FullPath END DESC;";

            var parameters = new
            {
                NameFilter = nameFilter,
                GroupNameFilter = groupNameFilter,
                GroupIdFilter = groupIdFilter,
                SortBy = sortBy?.ToLower(),
                SortOrder = sortOrder ? 1 : 0,
                IncludeInactive = includeInactive ? 1 : 0
            };

            var result = await connection.QueryAsync<ProductResponseDTO>(sql, parameters);
            return result;
        }

        public async Task AddProduct(ProductRequestDTO dto)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync("AddProduct", new { dto.Name, dto.Price, dto.GroupID }, commandType: CommandType.StoredProcedure);
        }

        public async Task ChangeProductStatus(int productId)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync("ChangeProductStatus", new { ProductId = productId }, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteProduct(int productId)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync("DeleteProduct", new { ProductId = productId }, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<GroupResponseDTO>> GetGroups(int? parentId, string? sortBy, bool sortOrder)
        {
            using var conn = new SqlConnection(_connectionString);

            var sql = new StringBuilder("SELECT ID, Name, CASE WHEN EXISTS (SELECT 1 FROM ProductGroups c WHERE c.ParentID = g.ID) THEN 1 ELSE 0 END AS HasChildren FROM ProductGroups g");

            if (parentId.HasValue)
                sql.Append(" WHERE ParentID = @ParentID");
            else
                sql.Append(" WHERE ParentID IS NULL");

            if (!string.IsNullOrEmpty(sortBy) && sortBy.ToLower() == "name")
                sql.Append(sortOrder ? " ORDER BY Name ASC" : " ORDER BY Name DESC");

            return await conn.QueryAsync<GroupResponseDTO>(sql.ToString(), new { ParentID = parentId });
        }

        public async Task AddGroup(GroupRequestDTO dto)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync("AddGroup", new { dto.Name, dto.ParentId }, commandType: CommandType.StoredProcedure);
        }
    }
}
