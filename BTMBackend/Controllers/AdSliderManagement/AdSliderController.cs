using BTMBackend.Data.Repos;
using BTMBackend.Dtos.AdSliderDto;
using BTMBackend.Dtos.CategoryDto;
using BTMBackend.Dtos.OrderDto;
using BTMBackend.Dtos.PublicDto;
using BTMBackend.Models;
using BTMBackend.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BTMBackend.Controllers.AdSliderManagement
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Adminstrator")]
    public class AdSliderController(IAdSliderRepo adSliderRepo, UploadFileService fileService) : ControllerBase
    {
        private readonly IAdSliderRepo _adSliderRepo = adSliderRepo;
        private readonly UploadFileService _fileService = fileService;
        private readonly Messages ms = new();


        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] CreateAdSliderRequestDto requestDto)
        {
            if (User.Identity is not ClaimsIdentity identity)
            {
                return Unauthorized();
            }

            var userIdClaim = identity.FindFirst(ClaimTypes.Name);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = Convert.ToInt32(userIdClaim.Value);

            var pathAr = await _fileService.UploadFile(requestDto.FileAr);
            var pathEn = await _fileService.UploadFile(requestDto.FileEn);

            if (pathAr == null || pathEn == null)
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.FailedAr,
                    MessageEn = ms.FailedEn
                });
            }

            var result = await _adSliderRepo.Create(new AdSlider()
            {
                Title = requestDto.Title,
                ImageArPath = pathAr,
                ImageEnPath = pathEn,
                UserId = userId
            });

            if (!result)
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.FailedAr,
                    MessageEn = ms.FailedEn
                });
            }

            return Ok(new MessageDto()
            {
                MessageAr = ms.AddedSuccessfullyAr,
                MessageEn = ms.AddedSuccessfullyEn
            });
        }


        [HttpGet("GetAll")]
        public async Task<ActionResult<GetAllSliderResponseDto>> GetAll(int page = 1, string query = "")
        {
            var result = query == null ? await _adSliderRepo.GetAll() : await _adSliderRepo.GetAllByTitle(query);
            if (result != null && result.Any())
            {
                var list = PagedList<GetAllSliderResponseDto>.ToPagedList(result, page, 10);
                return Ok(new { Data = list, Pagination = list.Paganation });
            }
            return Ok(new MessageDto
            {
                MessageAr = ms.NotFoundAr,
                MessageEn = ms.NotFoundEn,
            }); 
        }


        [HttpGet("GetDetailsById/{sliderId}")]
        public async Task<ActionResult<GetSliderDetailsByIdResponseDto>> GetDetailsById(int sliderId)
        {
            var result = await _adSliderRepo.GetDetailsById(sliderId);

            if (result == null)
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.NotFoundAr,
                    MessageEn = ms.NotFoundEn
                });
            }

            return Ok(result);
        }


        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromForm] UpdateSliderRequestDto requestDto)
        {
            if (User.Identity is not ClaimsIdentity identity)
            {
                return Unauthorized();
            }

            var userIdClaim = identity.FindFirst(ClaimTypes.Name);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = Convert.ToInt32(userIdClaim.Value);

            var slider = await _adSliderRepo.GetById(requestDto.SliderId);

            if (slider == null)
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.NotFoundAr,
                    MessageEn = ms.NotFoundEn
                });
            }

            await _fileService.DeleteFileAsync(slider.ImageArPath);
            await _fileService.DeleteFileAsync(slider.ImageEnPath);

            slider.Title = requestDto.Title;
            slider.ImageArPath = await _fileService.UploadFile(requestDto.FileAr);
            slider.ImageEnPath = await _fileService.UploadFile(requestDto.FileEn);
            slider.UpdatedAt = DateTime.Now;
            slider.UserId = userId;
            slider.IsActive = requestDto.IsActive;

            var result = await _adSliderRepo.Update(slider);

            if (!result)
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.FailedAr,
                    MessageEn = ms.FailedEn
                });
            }

            return Ok(new MessageDto()
            {
                MessageAr = ms.UpdatedSuccessfullyAr,
                MessageEn = ms.UpdatedSuccessfullyEn
            });
        }


        [HttpPut("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus(UpdateStatusRequestDto requestDto)
        {
            var result = await _adSliderRepo.UpdateStatus(requestDto.Id);
            if (!result)
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.FailedAr,
                    MessageEn = ms.FailedEn
                });
            }

            return Ok(new MessageDto()
            {
                MessageAr = ms.UpdatedSuccessfullyAr,
                MessageEn = ms.UpdatedSuccessfullyEn
            });
        }


        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(GetSliderIdRequestDto requestDto)
        {
            var slider = await _adSliderRepo.GetById(requestDto.SliderId);
            if(slider == null)
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.NotFoundAr,
                    MessageEn = ms.NotFoundEn
                });
            }

            await _fileService.DeleteFileAsync(slider.ImageArPath);
            await _fileService.DeleteFileAsync(slider.ImageEnPath);

            var result = await _adSliderRepo.Delete(slider);

            if (!result)
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.FailedAr,
                    MessageEn = ms.FailedEn
                });
            }

            return Ok(new MessageDto()
            {
                MessageAr = ms.DeletedSuccessfullyAr,
                MessageEn = ms.DeletedSuccessfullyEn
            });
        }

        [AllowAnonymous]
        [HttpGet("GetWebSliders")]
        public async Task<ActionResult<GetAllSlidersWebResponseDto>> GetWebSliders()
        {
            var result = await _adSliderRepo.GetAllActive();

            return Ok(result);
        }
    }
}
