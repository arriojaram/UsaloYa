using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsaloYa.Dto;

namespace UsaloYa.Services.interfaces
{
    public interface ICategoryService
    {
        Task<List<ProductCategoryDto>> GetAll4List(int companyId, string keyword);
    }
}
