using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOModels.ProductDTOs
{
    public class ProductResponseDTO
    {
        public int ProductID { get; init; }
        public string Name { get; init; }
        public double Price { get; init; }
        public string GroupName { get; init; }
    }
}