﻿using CodePulse.API.Models.Domain;
using CodePulse.API.Models.DTO;
using CodePulse.API.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodePulse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostsController : ControllerBase
    {
        private readonly IBlogPostRepository blogPostRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IGenericRepository<BlogPost> genericBlogPost;

        public BlogPostsController(IBlogPostRepository blogPostRepository, ICategoryRepository categoryRepository,IGenericRepository<BlogPost> genericBlogPost)
        {
            this.blogPostRepository = blogPostRepository;
            this.categoryRepository = categoryRepository;
            this.genericBlogPost = genericBlogPost;
        }
        [HttpPost]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> CreateBlogPost([FromBody] CreateBlogPostRequestDto request)
        {
            var blogPost = new BlogPost
            {
                Title = request.Title,
                Author = request.Author,
                ShortDescription = request.ShortDescription,
                Content = request.Content,
                UrlHandle = request.UrlHandle,
                FeaturedImageUrl = request.FeaturedImageUrl,
                IsVisible = request.IsVisible,
                PublishedDate = request.PublishedDate,
                Categories = new List<Category>()
            };
            //Here we are itterating through the create request guid of category and check if it valid or not with the help of category repository.If valid then add founded category in blogpost domain model
            foreach (var categoryGuid in request.Categories)
            {
                var existingCategory = await categoryRepository.GetByIdAsync(categoryGuid);
                
                if (existingCategory != null)
                {
                    blogPost.Categories.Add(existingCategory);
                }
            }
            //blogPost = await blogPostRepository.CreateAsync(blogPost);
            blogPost = await genericBlogPost.CreateAsync(blogPost);

            //Convert to DTO
            var response = new BlogPostDto
            {
                Id = blogPost.Id,
                Title = blogPost.Title,
                Author = blogPost.Author,
                UrlHandle = blogPost.UrlHandle,
                ShortDescription = blogPost.ShortDescription,
                Content = blogPost.Content,
                PublishedDate = blogPost.PublishedDate,
                IsVisible = blogPost.IsVisible,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                Categories = blogPost.Categories.Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlHandle = x.UrlHandle,
                }).ToList()
            };
            return Ok(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllBlogPosts()
        {
            var blogPosts = await genericBlogPost.GetAllAsync();
            //Convert to DTO
            var response = new List<BlogPostDto>();
            foreach (var blogPost in blogPosts)
            {
                response.Add(new BlogPostDto
                {
                    Id = blogPost.Id,
                    Title = blogPost.Title,
                    Author = blogPost.Author,
                    UrlHandle = blogPost.UrlHandle,
                    ShortDescription = blogPost.ShortDescription,
                    Content = blogPost.Content,
                    PublishedDate = blogPost.PublishedDate,
                    IsVisible = blogPost.IsVisible,
                    FeaturedImageUrl = blogPost.FeaturedImageUrl,
                    Categories = blogPost.Categories.Select(x => new CategoryDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        UrlHandle = x.UrlHandle,
                    }).ToList()
                });
            }
            return Ok(response);
        }
        //GET
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetBlogPostById([FromRoute] Guid id)
        {
            //Get the blogpost from repo
            //var blogPost = await blogPostRepository.GetByIdAsync(id);
            var blogPost = await genericBlogPost.GetByIdAsync(id);
            if (blogPost == null)
            {
                return NotFound();
            }
            //Convert to DTO
            var response = new BlogPostDto
            {
                Id = blogPost.Id,
                Title = blogPost.Title,
                Author = blogPost.Author,
                UrlHandle = blogPost.UrlHandle,
                ShortDescription = blogPost.ShortDescription,
                Content = blogPost.Content,
                PublishedDate = blogPost.PublishedDate,
                IsVisible = blogPost.IsVisible,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                Categories = blogPost.Categories.Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlHandle = x.UrlHandle,
                }).ToList()
            };
            return Ok(response);
        }

        //GET:{apiBaseUrl}/api/blogPosts/{urlHandle}
        [HttpGet]
        [Route("{urlHandle}")]
        public async Task<IActionResult> GetBlogPostByUrlHandle([FromRoute] string urlHandle)
        {
            //Get blogPost details from repository
            var blogPost = await blogPostRepository.GetByUrlHandleAsync(urlHandle);
            if (blogPost == null)
            {
                return NotFound();
            }
            //Convert to DTO
            var response = new BlogPostDto
            {
                Id = blogPost.Id,
                Title = blogPost.Title,
                Author = blogPost.Author,
                UrlHandle = blogPost.UrlHandle,
                ShortDescription = blogPost.ShortDescription,
                Content = blogPost.Content,
                PublishedDate = blogPost.PublishedDate,
                IsVisible = blogPost.IsVisible,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                Categories = blogPost.Categories.Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlHandle = x.UrlHandle,
                }).ToList()
            };
            return Ok(response);
        }

        //Put{id}
        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> UpdateBlogPostById([FromRoute] Guid id, UpdateBlogPostRequestDto request)
        {
            //DTO to Domain
            var blogPost = new BlogPost
            {
                Id = id,
                Title = request.Title,
                Author = request.Author,
                ShortDescription = request.ShortDescription,
                Content = request.Content,
                UrlHandle = request.UrlHandle,
                FeaturedImageUrl = request.FeaturedImageUrl,
                IsVisible = request.IsVisible,
                PublishedDate = request.PublishedDate,
                Categories = new List<Category>()
            };
            foreach (var categoryGuid in request.Categories)
            {
                var existingCategory = await categoryRepository.GetByIdAsync(categoryGuid);
                if (existingCategory != null)
                {
                    blogPost.Categories.Add(existingCategory);
                }
            }
            //Call repo to update domain
            //var UpdatedBlogPost = await blogPostRepository.UpdateAsync(blogPost);
            var UpdatedBlogPost = await genericBlogPost.UpdateAsync(blogPost);
            if (UpdatedBlogPost == null)
            {
                return NotFound();
            }

            var response = new BlogPostDto
            {
                Id = UpdatedBlogPost.Id,
                Title = UpdatedBlogPost.Title,
                Author = UpdatedBlogPost.Author,
                UrlHandle = UpdatedBlogPost.UrlHandle,
                ShortDescription = UpdatedBlogPost.ShortDescription,
                Content = UpdatedBlogPost.Content,
                PublishedDate = UpdatedBlogPost.PublishedDate,
                IsVisible = UpdatedBlogPost.IsVisible,
                FeaturedImageUrl = UpdatedBlogPost.FeaturedImageUrl,
                Categories = UpdatedBlogPost.Categories.Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlHandle = x.UrlHandle,
                }).ToList()
            };
            return Ok(response);
        }
        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> DeleteBlogPost(Guid id)
        {
           // var deletedBlogPost = await blogPostRepository.DeleteAsync(id);
            var deletedBlogPost = await genericBlogPost.DeleteAsync(id);
            if (deletedBlogPost == null) { return NotFound(); }
            //Domain to DTo
            var response = new BlogPostDto
            {
                Id = deletedBlogPost.Id,
                Title = deletedBlogPost.Title,
                Author = deletedBlogPost.Author,
                UrlHandle = deletedBlogPost.UrlHandle,
                ShortDescription = deletedBlogPost.ShortDescription,
                Content = deletedBlogPost.Content,
                PublishedDate = deletedBlogPost.PublishedDate,
                IsVisible = deletedBlogPost.IsVisible,
                FeaturedImageUrl = deletedBlogPost.FeaturedImageUrl

            };
            return Ok(response);
        }
    }
}
