﻿using Catalog.Application.Categories.Dtos;
using Catalog.Application.Categories.Extensions;
using Catalog.Application.Services.Categories.Abstraction;
using Catalog.Domain;
using Microsoft.Extensions.Options;
using Shared.Results;
using System.Text.Json;

namespace Catalog.Application.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly HttpClient _httpClient;

        public CategoryService(HttpClient httpClient)
        {
            this._httpClient = httpClient;
        }
        public async Task<Result<IEnumerable<CategoryDto>>> GetCategories()
        { 
            string endpoint = "https://dummyjson.com/products/categories?delay=5000"; 

            var response = await _httpClient.GetAsync(endpoint);
            string responseJson = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var users = JsonSerializer.Deserialize<IEnumerable<Category>>(responseJson, options);

                return Result<IEnumerable<CategoryDto>>.Success(users.Select(u => u.ToDto()));
            }

            return Result<IEnumerable<CategoryDto>>.Failure([responseJson]);
        }
    }
}
