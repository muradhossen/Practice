using Catalog.Application.Categories.Dtos;
using Shared.Results;

namespace Catalog.Application.Services.Categories.Abstraction
{
    public interface ICategoryService
    {
        Task<Result<IEnumerable<CategoryDto>>> GetCategoriesAsync();
    }
}
