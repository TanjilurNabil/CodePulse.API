using CodePulse.API.Models.Domain;
using CodePulse.API.Models.DTO;
using CodePulse.API.Repositories.Implementation;
using CodePulse.API.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodePulse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository imageRepository;
        private readonly IConfiguration configuration;

        public ImagesController(IImageRepository imageRepository,IConfiguration configuration)
        {
            this.imageRepository = imageRepository;
            this.configuration = configuration;
        }
        [HttpGet]
        public async Task<IActionResult> GelAllImage()
        {
            var images = await imageRepository.GetAll();
            var response = new List<BlogImageDto>();
            foreach (var image in images)
            {
                response.Add(new BlogImageDto
                {
                    Id = image.Id,
                    Title = image.Title,
                    DateCreated = image.DateCreated,
                    FileName = image.FileName,
                    FileExtension = image.FileExtension,
                    Url = image.Url,
                });
            }
            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file, [FromForm] string fileName, [FromForm] string title)
        {
            ValidateFileUpload(file);
            if (ModelState.IsValid)
            {
                //File Upload
                var blogImage = new BlogImage
                {
                    FileExtension = Path.GetExtension(file.FileName).ToLower(),
                    Title = title,
                    FileName = fileName,
                    DateCreated = DateTime.Now,
                };
                blogImage = await imageRepository.Upload(file, blogImage);
                //Convert to DTO
                var response = new BlogImageDto
                {
                    Id = blogImage.Id,
                    Title = blogImage.Title,
                    DateCreated = blogImage.DateCreated,
                    FileName = blogImage.FileName,
                    FileExtension = blogImage.FileExtension,
                    Url = blogImage.Url,
                };
                return Ok(response);
            }

            return BadRequest(ModelState);
        }

        private void ValidateFileUpload(IFormFile file)
        {
           // var allowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };
            var allowedExtensions = configuration.GetSection("AllowedExtensions").Get<string[]>();
            if (allowedExtensions is not null)
            {
                if (!allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
                {
                    ModelState.AddModelError("file", "Unsupported file format");
                }
            }
            
            if (file.Length > 10485760)
            {
                ModelState.AddModelError("file", "File size cannot be more than 10MB");

            }
        }
    }
}
