using AutoMapper;
using BTMBackend.Data.Repos;
using BTMBackend.Dtos.AboutUsDto;
using BTMBackend.Dtos.CategoryDto;
using BTMBackend.Dtos.Product;
using BTMBackend.Dtos.PublicDto;
using BTMBackend.Models;
using BTMBackend.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace BTMBackend.Controllers.AboutUsManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class AboutUsController(IAboutUsRepocs aboutUsRepo, IMapper mapper) : ControllerBase
    {
        private readonly IAboutUsRepocs _aboutUsRepo = aboutUsRepo;
        private readonly IMapper _mapper = mapper;
        readonly Messages message = new();

        [Authorize(Roles = "Adminstrator"), HttpPost("Create")]
        public async Task<IActionResult> Create(CreateAboutUsRequestDto request)
        {
            #region Check Token Data

            var userId = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            #endregion

            if (string.IsNullOrEmpty(request.NameAr) || string.IsNullOrEmpty(request.DescriptionAr) || string.IsNullOrEmpty(request.NameEn) || string.IsNullOrEmpty(request.DescriptionEn))
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.EmptyValueAr,
                    MessageEn = message.EmptyValueEn,
                });
            }
            

            var aboutUs = _mapper.Map<AboutUs>(request);
            int userIdValue;
            int.TryParse(userId, out userIdValue);
            aboutUs.UserId = userIdValue;
            if (aboutUs != null)
                await _aboutUsRepo.CreateAsync(aboutUs);
            if (!await _aboutUsRepo.SaveChangesAsync())
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.FailedAr,
                    MessageEn = message.FailedEn,
                });
            }
            return Ok();
        }
        [Authorize(Roles = "Adminstrator"), HttpPut("Update")]
        public async Task<IActionResult> Update(UpdateAboutUsRequestDto request)
        {
            

            if (request.Id.ToString() == null)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.EmptyValueAr,
                    MessageEn = message.EmptyValueEn
                });
            }

            var result = await _aboutUsRepo.GetByIdAsync(request.Id);
            if (result == null) return NotFound();
            if (result != null)
            {
                result.NameAr = request.NameAr;
                result.DescriptionAr = request.DescriptionAr;
                result.DescriptionEn = request.DescriptionEn;
                result.NameEn = request.NameEn;
                result.IsActive=request.IsActive;
                result.UpdatedAt = DateTime.Now;
                if (result != null)
                _aboutUsRepo.Update(result);
            }
            else return BadRequest(new MessageDto
            {
                MessageAr = message.FailedAr,
                MessageEn = message.FailedEn,
            });
            if (!await _aboutUsRepo.SaveChangesAsync())
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.FailedAr,
                    MessageEn = message.FailedEn,
                });
            }
            return Ok();
        }
        [Authorize(Roles = "Adminstrator"), HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(int page = 1,string query="")
        {
            var result = query == null ? await _aboutUsRepo.GetAllAsync() : await _aboutUsRepo.SearchByName(query);

            if (result != null && result.Any())
            {
                var list = PagedList<GetAllAboutUSResponseDto>.ToPagedList(result, page, 10);
                return Ok(new { Data = list, Pagination = list.Paganation });
            }

            return Ok(new MessageDto
            {
                MessageAr = message.NotFoundAr,
                MessageEn = message.NotFoundEn,
            });
        }

        [HttpGet("GetForHPageome")]
        public async Task<IActionResult> GetForHPageome()
        {
            var result = await _aboutUsRepo.GetAsync();

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

        //[Authorize(Roles = "Adminstrator"), HttpPost("SearchByName")]
        //public async Task<IActionResult> SearchByName(SearchAboutUsRequestDto request)
        //{
        //    var result = await _aboutUsRepo.SearchByName(request.Name);
        //    if (result != null && result.Any())
        //    {
        //        var list = PagedList<GetAllAboutUSResponseDto>.ToPagedList(result, request.Page, 10);
        //        Response.Headers.Append("X-Pagination", System.Text.Json.JsonSerializer.Serialize(list.Paganation));
        //        return Ok(list);
        //    }
        //    return Ok(new MessageDto
        //    {
        //        MessageAr = message.NotFoundAr,
        //        MessageEn = message.NotFoundEn,
        //    }); ;

        //}
        [Authorize(Roles = "Adminstrator"), HttpDelete("Delete")]
        public async Task<IActionResult> Delete(DeleteAboutUsRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Id.ToString()))
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.EmptyValueAr,
                    MessageEn = message.EmptyValueEn
                });
            }
            var aboutUs = await _aboutUsRepo.GetByIdAsync(request.Id);

            if (aboutUs == null)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.FailedToDeleteAr,
                    MessageEn = message.FailedToDeleteEn,
                });
            }
            _aboutUsRepo.Delete(aboutUs);

            if (!await _aboutUsRepo.SaveChangesAsync())
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.FailedAr,
                    MessageEn = message.FailedEn,
                });
            }
            return Ok();
        }

    }
}
