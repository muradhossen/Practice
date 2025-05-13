using Catalog.Application.Categories.Dtos;
using Catalog.Application.Services.Categories.Abstraction;
using Shared.Results;
using StackExchange.Redis;
using System.Text.Json;

namespace Catalog.Application.Services.Categories
{
    public class CategoryCachingService : ICategoryCachingService
    {
        private readonly IDatabase _db;
        private readonly ICategoryService _categoryService;

        public CategoryCachingService(ConnectionMultiplexer redis
            , ICategoryService categoryService)
        {
            _db = redis.GetDatabase();
            this._categoryService = categoryService;
        }

        public async Task SetCategoriesAsync(IEnumerable<CategoryDto> categories)
        {
            await _db.HashSetAsync("categories", "all", JsonSerializer.Serialize(categories));
        }

        public async Task<Result<IEnumerable<CategoryDto>>> GetCategoriesAsync()
        { 
            if (_db.HashExists("categories", "all"))
            {
                var categories = JsonSerializer.Deserialize<IEnumerable<CategoryDto>>(_db.HashGet("categories", "all"));

                return Result<IEnumerable<CategoryDto>>.Success(categories);

            }
            var result = await _categoryService.GetCategoriesAsync();

            if (result.IsSuccess)
            {
                await SetCategoriesAsync(result.Data);
                return result; 
            }

            return Result<IEnumerable<CategoryDto>>.Failure(result.Errors);

        }
    }
}
