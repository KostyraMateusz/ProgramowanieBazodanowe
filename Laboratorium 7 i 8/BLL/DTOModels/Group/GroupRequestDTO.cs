using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOModels.GroupDTOs
{
    public class GroupRequestDTO
    {
        public string Name { get; set; }
        public int? ParentId { get; set; }
    }
}
