using BTMBackend.Data.Repos;
using BTMBackend.Dtos.AuthorityDto;
using BTMBackend.Dtos.CustomerDto;
using BTMBackend.Models;
using BTMBackend.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BTMBackend.Controllers.AuthortyManagement
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Adminstrator")]
    public class AuthorityController(IAuthorityRepo authorityRepo) : ControllerBase
    {
        readonly Messages ms = new();
        private readonly IAuthorityRepo _authorityRepo = authorityRepo;

        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole(CreateRoleRequestDto requestDto)
        {
            var checkTitle = await _authorityRepo.IsRoleTitleExist(requestDto.TitleAr, requestDto.TitleEn);
            if (checkTitle)
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.AlreadyExistAr,
                    MessageEn = ms.AlreadyExistEn
                });
            }

            _authorityRepo.CreateRole(new Role()
            {
                TitleAr = requestDto.TitleAr,
                TitleEn = requestDto.TitleEn,
                IsActive = true,
            });

            if (!await _authorityRepo.SaveChanges())
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

        [HttpPut("UpdateRole")]
        public async Task<IActionResult> UpdateRole(UpdateRoleRequestDto requestDto)
        {
            var getRoleById = await _authorityRepo.GetRoleByIdAsync(requestDto.Id);
            if (getRoleById == null)
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.NotFoundAr,
                    MessageEn = ms.NotFoundEn
                });
            }

            if (getRoleById.TitleEn != requestDto.TitleAr)
            {
                var checkTitle = await _authorityRepo.IsRoleTitleExist(requestDto.TitleAr, requestDto.TitleEn);
                if (checkTitle)
                {
                    return BadRequest(new MessageDto()
                    {
                        MessageAr = ms.AlreadyExistAr,
                        MessageEn = ms.AlreadyExistEn
                    });
                }
            }

            getRoleById.TitleAr = requestDto.TitleAr;
            getRoleById.TitleEn = requestDto.TitleEn;
            getRoleById.UpdatedAt = DateTime.Now;

            _authorityRepo.UpdateRole(getRoleById);
            if (!await _authorityRepo.SaveChanges())
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.FailedAr,
                    MessageEn = ms.FailedEn
                });
            }

            return Ok(new MessageDto
            {
                MessageAr = ms.UpdatedSuccessfullyAr,
                MessageEn = ms.UpdatedSuccessfullyEn
            });
        }

        [HttpPut("ChangeRoleStatus/{RoleId}")]
        public async Task<IActionResult> ChangeRoleStatus(int roleId)
        {
            var getRoleById = await _authorityRepo.GetRoleByIdAsync(roleId);
            if (getRoleById == null)
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.NotFoundAr,
                    MessageEn = ms.NotFoundEn
                });
            }

            getRoleById.IsActive = getRoleById.IsActive == true ? false : true;
            _authorityRepo.UpdateRole(getRoleById);
            if (!await _authorityRepo.SaveChanges())
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.FailedAr,
                    MessageEn = ms.FailedEn
                });
            }

            return Ok(new MessageDto
            {
                MessageAr = ms.UpdatedSuccessfullyAr,
                MessageEn = ms.UpdatedSuccessfullyEn
            });
        }

        [HttpGet("GetAllRoles")]
        public async Task<ActionResult<GetAllRoleResponseDto>> GetAllRoles(int page = 1, int row = 10)
        {
            var result = await _authorityRepo.GetAllRoles();

            if (!result.Any())
            {
                return NotFound(new MessageDto()
                {
                    MessageAr = ms.NotFoundAr,
                    MessageEn = ms.NotFoundEn
                });
            }

            var list = PagedList<GetAllRoleResponseDto>.ToPagedList(result, page, row);
            Response.Headers.Append("X-Pagination", System.Text.Json.JsonSerializer.Serialize(list.Paganation));
            return Ok(list);
        }


        [HttpGet("GetEmployeeRoles")]
        public async Task<ActionResult<GetAllRoleResponseDto>> GetEmployeeRoles()
        {
            var result = await _authorityRepo.GetAllEmployeeRoles();

            if (result.Count == 0)
            {
                return NotFound(new MessageDto()
                {
                    MessageAr = ms.NotFoundAr,
                    MessageEn = ms.NotFoundEn
                });
            }

            return Ok(result);
        }
    }
}
