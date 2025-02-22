using Catalog.Application.Categories.Dtos;
using Catalog.Domain;

namespace Catalog.Application.Categories.Extensions
{
    public static class MapperExtensions
    {
        public static CategoryDto ToDto(this Category category) => new(
            category.Slug,
            category.Name,
            category.Url
        );
    }
}
