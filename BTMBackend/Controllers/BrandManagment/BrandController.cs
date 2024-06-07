using AutoMapper;
using BTMBackend.Data.Repos;
using BTMBackend.Dtos.BrandDto;
using BTMBackend.Models;
using BTMBackend.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BTMBackend.Controllers.BrandManagment
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController(IBrandRepo brandRepo, IMapper mapper, UploadFileService uploadFileService) : ControllerBase
    {
        private readonly IBrandRepo _brandRepo = brandRepo;
        private readonly UploadFileService _uploadFileService = uploadFileService;
        private readonly IMapper _mapper = mapper;
        private readonly Messages message = new();


        [Authorize(Roles = "Adminstrator"), HttpPost("CreateBrand")]
        public async Task<IActionResult> CreateBrand([FromForm] CreateBrandRequestDto request)
        {
            #region Check Token Data

            var userId = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            #endregion



            #region Validation

            if (string.IsNullOrEmpty(request.NameAr) || string.IsNullOrEmpty(request.NameEn))
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.EmptyValueAr,
                    MessageEn = message.EmptyValueEn,
                });
            }

            var brandExists = await _brandRepo.GetBrandByNameForAdd(request.NameAr, request.NameEn);
            if (brandExists)
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = message.AlreadyExistAr,
                    MessageEn = message.AlreadyExistEn,
                });
            }

            #endregion
            #region File Upload

            string filePath;
            try
            {
                filePath = await _uploadFileService.UploadFile(request.File);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            #endregion

            #region Create Brand

            var brand = _mapper.Map<Brand>(request);
            brand.Logo = filePath;

            int.TryParse(userId, out int userIdValue);
            brand.UserId = userIdValue;

            await _brandRepo.CreateAsync(brand);

            if (!await _brandRepo.SaveChangesAsync())
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.FailedAr,
                    MessageEn = message.FailedEn,
                });
            }

            #endregion

            return Ok();
        }

        [HttpGet("GetBrandForHomePage")]
        public async Task<IActionResult> GetBrandForHomePage()
        {
            var result = await _brandRepo.GetAllBrandsFroHomePa();

            if (result != null && result.Any())
            {

                return Ok(result);
            }

            return Ok(new MessageDto
            {
                MessageAr = message.NotFoundAr,
                MessageEn = message.NotFoundEn,
            });
        }
    }
}
