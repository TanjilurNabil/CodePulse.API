using CodePulse.API.Data;
using CodePulse.API.Models.Domain;
using CodePulse.API.Models.DTO;
using CodePulse.API.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodePulse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IGenericRepository<Category> genericCategory;

        public CategoriesController(ICategoryRepository categoryRepository,IGenericRepository<Category> genericCategory)
        {
            this.categoryRepository = categoryRepository;
            this.genericCategory = genericCategory;
        }

        [HttpPost]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequestDto request)
        {
            //Map DTO to Domain Model
            var category = new Category
            {
                Name = request.Name,
                UrlHandle = request.UrlHandle
            };
            //await categoryRepository.CreateAsync(category);
            //await categoryRepository.CreateAsync(category);
            await genericCategory.CreateAsync(category);
            //Domain model to DTO
            var response = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                UrlHandle = category.UrlHandle
            };
            return Ok(response);
        }
        //GET:https://localhost:7176/api/Categories
        [HttpGet]
        
        public async Task<IActionResult> GetAllCategory()
        {
            //var categoryObj = await categoryRepository.GetAllAsync();
            
            var categoryObj = await genericCategory.GetAllAsync();
            //Map response to DTo
            var response = new List<CategoryDto>();
            foreach (var category in categoryObj)
            {
                response.Add(new CategoryDto { Id = category.Id, Name = category.Name, UrlHandle = category.UrlHandle });
            }
            return Ok(response);

        }
        //GET:https://localhost:7176/api/Categories/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetCategoryById([FromRoute] Guid id)
        {
            //var existingCategory = await categoryRepository.GetById(id);
            var existingCategory = await genericCategory.GetByIdAsync(id);
            if (existingCategory is null)
            {
                return NotFound();
            }
            var response = new CategoryDto
            {
                Id = existingCategory.Id,
                Name = existingCategory.Name,
                UrlHandle = existingCategory.UrlHandle
            };
            return Ok(response);

        }
        //PUT:https://localhost:7176/api/Categories/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> EditCategory([FromRoute] Guid id, UpdateCategoryRequestDto request)
        {
            //Map request to domain model
            var category = new Category
            {
                Id = id,
                Name = request.Name,
                UrlHandle = request.UrlHandle
            };
            //category = await categoryRepository.UpdateAsync(category);
            category = await genericCategory.UpdateAsync(category);
            if (category is null)
            {
                return NotFound();
            }
            //Convert Domain to DTO
            var response = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                UrlHandle = category.UrlHandle
            };
            return Ok(response);
        }
        //Delete:https://localhost:7176/api/Categories/{id}
        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> DeleteCategory([FromRoute] Guid id)
        {
            //var category = await categoryRepository.DeleteAsync(id);
            var category = await genericCategory.DeleteAsync(id);
            if (category is null)
            {
                return NotFound();
            }
            var respose = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                UrlHandle = category.UrlHandle
            };
            return Ok(respose);
        }
    }
}
