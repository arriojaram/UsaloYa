using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsaloYa.Dto;
using UsaloYa.Library.Models;
using Microsoft.EntityFrameworkCore;
using UsaloYa.Services.interfaces;

namespace UsaloYa.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly DBContext _dBContext;

        public CategoryService(DBContext dBContext)
        {
            _dBContext = dBContext;
        }

        public async Task<List<ProductCategoryDto>> GetAll4List(int companyId, string keyword) 
        {
            
                var categories = keyword == "-1" ?
                    await _dBContext.ProductCategories
                                            .Where(c => c.CompanyId == companyId)
                                            .OrderBy(u => u.Name)
                                            .ToListAsync()
                    :
                    await _dBContext.ProductCategories
                                            .Where(c => c.CompanyId == companyId
                                                        && c.Name.Contains(keyword))
                                            .OrderBy(u => u.Name)
                                            .ToListAsync();

                return categories.Select(c => new ProductCategoryDto
                {
                    CategoryId = c.CategoryId,
                    Description = c.Description ?? "",
                    Name = c.Name,
                    CompanyId = c.CompanyId
                }).ToList();
 
        }
    }
}
