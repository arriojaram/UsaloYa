﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsaloYa.Dto;

namespace UsaloYa.Services.interfaces
{
    public interface IProductCategoryService
    {
        Task<List<ProductCategoryDto>> GetAll4List(int companyId, string keyword);
        Task<ProductCategoryDto?> GetCategory(int categoryId, int companyId);
        Task<ProductCategoryDto?> GetCategoryByName(string categoryName, int companyId);
        Task<ProductCategoryDto?> SaveCategory(ProductCategoryDto dto);
        Task<bool> DeleteCategory(int categoryId, int companyId);

    }
}
