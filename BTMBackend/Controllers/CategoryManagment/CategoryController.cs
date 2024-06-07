using AutoMapper;
using BTMBackend.Data.Repos;
using BTMBackend.Dtos.CategoryDto;
using BTMBackend.Models;
using BTMBackend.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BTMBackend.Controllers.CategoryManagment
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(ICategoryRepo categoryRepo, IMapper mapper) : ControllerBase
    {
        private readonly ICategoryRepo _categoryRepo = categoryRepo;
        private readonly IMapper _mapper = mapper;
        readonly Messages message = new();

        [Authorize(Roles = "Adminstrator"), HttpPost("CreateCategory")]
        public async Task<IActionResult> CreateCategory(CreateCategoryRequestDto request)
        {
            #region Check Token Data

            var userId = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            #endregion

            if (request.NameAr == "" || request.NameAr == null || request.NameEn == "" || request.NameEn == null )
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.EmptyValueAr,
                    MessageEn = message.EmptyValueEn,
                });
            }
            var result = await _categoryRepo.GetCategoryByNameForAddAsync(request.NameAr,request.NameEn);
            if (result)
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = message.AlreadyExistAr,
                    MessageEn = message.AlreadyExistEn,
                });
            }

            var category = _mapper.Map<Category>(request);
            int userIdValue;
            int.TryParse(userId, out userIdValue);
            category.UserId = userIdValue;
            if (category != null)
                await _categoryRepo.CreateAsync(category);
            if (!await _categoryRepo.SaveChangesAsync())
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.FailedAr,
                    MessageEn = message.FailedEn,
                });
            }
            return Ok();
        }

        [Authorize(Roles = "Adminstrator"), HttpPut("UpdateCategory")]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryRequestDto request)
        {
            if (request.Id.ToString() == null)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.EmptyValueAr,
                    MessageEn = message.EmptyValueEn
                });
            }

            var result = await _categoryRepo.GetByIdAsync(request.Id);
            if (result == null) return NotFound();

            var existingItem = await _categoryRepo.GetCategoryByNameForUpdateAsync(request.NameAr,request.NameEn, request.Id);
            if (existingItem)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.AlreadyExistAr,
                    MessageEn = message.AlreadyExistEn
                });
            }

            result.NameEn = request.NameEn;
            result.NameAr = request.NameAr;
            result.IsActive = request.IsActive;

            
            if (result != null)
            {
                result.UpdatedAt = DateTime.Now;
                _categoryRepo.Update(result);
            }
            else return BadRequest(new MessageDto
            {
                MessageAr = message.FailedAr,
                MessageEn = message.FailedEn,
            });
            if (!await _categoryRepo.SaveChangesAsync())
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.FailedAr,
                    MessageEn = message.FailedEn,
                });
            }
            return Ok();
        }

        [Authorize(Roles = "Adminstrator"), HttpGet("GetAllCategories")]
        public async Task<IActionResult> GetAllCategories(int page=1, string query = "")
        {
            var result = query == null ? await _categoryRepo.GetAllAsync(): await _categoryRepo.SearchByName(query);
            if (result != null && result.Any())
            {
                var list = PagedList<GetAllCategoryResponceDto>.ToPagedList(result, page, 10);
                return Ok(new { Data = list, Pagination = list.Paganation });
            }
            return Ok(new MessageDto
            {
                MessageAr = message.NotFoundAr,
                MessageEn = message.NotFoundEn,
            }); ;

        }

        [AllowAnonymous]
        [HttpGet("AppGetCategories")]
        public async Task<ActionResult<AppGetAllCategoryResponseDto>> AppGetCategories()
        {
            var result = await _categoryRepo.AppGetAllAsync();
            return Ok(result);
        }

    }
}
