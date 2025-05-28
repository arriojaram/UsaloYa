using Microsoft.EntityFrameworkCore;
using UsaloYa.Dto;
using UsaloYa.Library.Models;

namespace UsaloYa.API.Services
{
    public class ProductCategoryService
    {
        private readonly DBContext _dBContext;
        public ProductCategoryService(DBContext dBContext) 
        {
            _dBContext = dBContext;
        }

        public async Task<ProductCategoryDto> SaveCategory(ProductCategoryDto categoryDto)
        {
            ProductCategory objectToSave;
            if (categoryDto.CategoryId == 0)
            {

                objectToSave = new ProductCategory
                {
                    Name = categoryDto.Name,
                    CategoryId = categoryDto.CategoryId,
                    CompanyId = categoryDto.CompanyId,
                    Description = categoryDto.Description
                };
                _dBContext.ProductCategories.Add(objectToSave);
            }
            else
            {
                objectToSave = await _dBContext.ProductCategories.FindAsync(categoryDto.CategoryId);
                if (objectToSave == null)
                    return null;

                objectToSave.Description = categoryDto.Description;
                objectToSave.Name = categoryDto.Name;

                _dBContext.Entry(objectToSave).State = EntityState.Modified;

            }

            await _dBContext.SaveChangesAsync();
            return new ProductCategoryDto()
                        {
                            Name = categoryDto.Name,
                            CategoryId = objectToSave.CategoryId,
                            CompanyId = categoryDto.CompanyId,
                            Description = categoryDto.Description
                        };
        }

        public async Task<ProductCategoryDto> GetCategory(int categoryId, int companyId)
        {
            var c = await _dBContext.ProductCategories
                .FirstOrDefaultAsync(u => u.CompanyId == companyId && u.CategoryId == categoryId);
            if (c == null)
                return null;

            var responseDto = new ProductCategoryDto()
            {
                CategoryId = c.CategoryId,
                Description = c.Description ?? "",
                Name = c.Name,
                CompanyId = companyId
            };

            return responseDto;
        }

        public async Task<ProductCategoryDto> GetCategoryByName(string categoryName, int companyId)
        {
            ProductCategoryDto categoryInfo;
            categoryName = categoryName.ToLower();

            var c = await _dBContext.ProductCategories
                .FirstOrDefaultAsync(u => u.CompanyId == companyId && u.Name.ToLower() == categoryName);

            if (c != null)
            {
                categoryInfo = new ProductCategoryDto()
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name
                };
                return categoryInfo;
            }
            
            var prodCatDto = new ProductCategoryDto() 
            {
                CategoryId = 0,
                Name = categoryName,
                CompanyId = companyId
            };

            categoryInfo = await SaveCategory(prodCatDto);
            return categoryInfo;
        }

    }
}
