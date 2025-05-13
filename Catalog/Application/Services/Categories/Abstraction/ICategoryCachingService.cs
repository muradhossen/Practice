using Catalog.Application.Categories.Dtos;
using Shared.Results;

namespace Catalog.Application.Services.Categories.Abstraction;

public interface ICategoryCachingService
{ 
    Task<Result<IEnumerable<CategoryDto>>> GetCategoriesAsync();
    Task SetCategoriesAsync(IEnumerable<CategoryDto> categories);
}
