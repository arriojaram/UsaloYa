using Microsoft.EntityFrameworkCore;
using UsaloYa.Dto;
using UsaloYa.Library.Models;
using UsaloYa.Services.interfaces;

namespace UsaloYa.Services
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly DBContext _dBContext;
        public ProductCategoryService(DBContext dBContext) 
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

        public async Task<ProductCategoryDto?> SaveCategory(ProductCategoryDto dto)
        {
            var exists = await _dBContext.ProductCategories
                .AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower()
                            && c.CompanyId == dto.CompanyId
                            && c.CategoryId != dto.CategoryId);

            if (exists)
                throw new InvalidOperationException("El nombre de la categoría ya existe");

            ProductCategory entity;

            if (dto.CategoryId == 0)
            {
                entity = new ProductCategory
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    CompanyId = dto.CompanyId
                };
                _dBContext.ProductCategories.Add(entity);
            }
            else
            {
                entity = await _dBContext.ProductCategories.FirstOrDefaultAsync(c => c.CategoryId == dto.CategoryId && c.CompanyId == dto.CompanyId);

                if (entity == null) return null;

                entity.Name = dto.Name;
                entity.Description = dto.Description;
            }

            await _dBContext.SaveChangesAsync();

            return new ProductCategoryDto
            {
                CategoryId = entity.CategoryId,
                Name = entity.Name,
                Description = entity.Description ?? "",
                CompanyId = entity.CompanyId
            };
        }

        public async Task<ProductCategoryDto?> GetCategory(int categoryId, int companyId)
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

        public async Task<ProductCategoryDto?> GetCategoryByName(string categoryName, int companyId)
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



        public async Task<bool> DeleteCategory(int categoryId, int companyId)
        {
            var category = await _dBContext.ProductCategories
                .FirstOrDefaultAsync(c => c.CompanyId == companyId && c.CategoryId == categoryId);

            if (category == null)
                return false;

            _dBContext.ProductCategories.Remove(category);
            await _dBContext.SaveChangesAsync();
            return true;
        }

    }
}
