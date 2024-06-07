
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using BTMBackend.Utilities;
using BTMBackend.Dtos.Product;
using BTMBackend.Data.Repos;


namespace BTMBackend.Controllers.ProductManagment
{
    [Route("api/[controller]")]
    [ApiController]
    public class testController(UploadFileService uploadFileService, IProductRepo productRepo) : ControllerBase
    {
        private readonly UploadFileService _uploadFileService = uploadFileService;
        private readonly IProductRepo _productRepo = productRepo;

        [HttpPost("UploadFile")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            try
            {
                string filePath = await _uploadFileService.UploadFile(file);
                return Ok("File uploaded successfully. Path: " + filePath);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("GetImageBytes")]
        public async Task<IActionResult> GetImageBytes(TestRequest request)
        {
            try
            {
                var path = await _productRepo.GetByIdAsync(request.Id);
                if(path != null)
                {
                    byte[] imageBytes = await _uploadFileService.ConvertFileToByteArrayAsync(path.ImageUrl);
                    return Ok(imageBytes);
                }
                else { return BadRequest("no"); }
                
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
        }
    }


}
