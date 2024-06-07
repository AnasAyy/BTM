using AutoMapper;
using BTMBackend.Data.Repos;
using BTMBackend.Dtos.CategoryDto;
using BTMBackend.Dtos.ContactUsDto;
using BTMBackend.Dtos.ExternalLinkDto;
using BTMBackend.Dtos.PublicDto;
using BTMBackend.Dtos.PublicListDto;
using BTMBackend.Models;
using BTMBackend.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace BTMBackend.Controllers.ExternalLinkManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExternalLinkController : ControllerBase
    {
        private readonly IExternalLinkRepo _externalLink;
        private readonly IMapper _mapper;
        Messages message = new Messages();

        public ExternalLinkController(IExternalLinkRepo externalLink, IMapper mapper)
        {
            _externalLink = externalLink;
            _mapper = mapper;
        }

        //[Authorize(Roles = "Adminstrator"), HttpPost("CreateExternalLink")]
        //public async Task<IActionResult> CreateExternalLink(AddExternalLinkRequestDto request)
        //{
        //    #region Check Token Data

        //    var userId = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
        //    if (string.IsNullOrEmpty(userId))
        //    {
        //        return Unauthorized();
        //    }

        //    #endregion

        //    if (string.IsNullOrEmpty(request.Name)|| string.IsNullOrEmpty(request.Link.ToString()) || string.IsNullOrEmpty(request.Type.ToString()))
        //    {
        //        return BadRequest(new MessageDto
        //        {
        //            MessageAr = message.EmptyValueAr,
        //            MessageEn = message.EmptyValueEn
        //        });
        //    }

        //    if (!await _externalLink.CheckyTypeAsync(request.Type))
        //    {
        //        return BadRequest(new MessageDto()
        //        {
        //            MessageAr = message.TypeNotExistsAr,
        //            MessageEn = message.TypeNotExistsEn,
        //        });
        //    }

        //    var item = await _externalLink.GetLinkForAddAsync(request.Name, request.Link);
        //    if (item)
        //    {
        //        return BadRequest(new MessageDto()
        //        {
        //            MessageAr = message.AlreadyExistAr,
        //            MessageEn = message.AlreadyExistEn,
        //        });
        //    }

        //    var link = _mapper.Map<ExternalLink>(request);
        //    int userIdValue;
        //    int.TryParse(userId, out userIdValue);
        //    link.UserId = userIdValue;
        //    if (link != null)
        //        await _externalLink.CreateAsync(link);
        //    if (!await _externalLink.SaveChangesAsync())
        //    {
        //        return BadRequest(new MessageDto
        //        {
        //            MessageAr = message.FailedAr,
        //            MessageEn = message.FailedEn,
        //        });
        //    }
        //    return Ok();

        //}

        //[Authorize(Roles = "Adminstrator"), HttpGet("GetTypes")]
        //public async Task<IActionResult> GetTypes()
        //{
        //    var result = await _externalLink.GetTypeAsync();
        //    if (result != null && result.Count != 0)
        //    {
        //        return Ok(result);
        //    }
        //    return NotFound();
        //}

        [Authorize(Roles = "Adminstrator"), HttpPut("UpdateExternalLink")]
        public async Task<IActionResult> UpdateExternalLink(UpdateExternalLinkRequestDto request)
        {
            if (request.Id.ToString() == null || string.IsNullOrEmpty(request.Link.ToString()))
            {
                return BadRequest(new MessageDto 
                { 
                    MessageAr = message.EmptyValueAr, 
                    MessageEn = message.EmptyValueEn 
                });
            }

            var result = await _externalLink.GetByIdAsync(request.Id);
            if (result == null) return NotFound();

            var existingItem = await _externalLink.GetLinkForUpdateAsync(request.Link, request.Id);
            if (existingItem)
            {
                return BadRequest(new MessageDto 
                { 
                    MessageAr = message.AlreadyExistAr,
                    MessageEn = message.AlreadyExistEn
                });
            }

            //if (!await _externalLink.CheckyTypeAsync(request.Type))
            //{
            //    return BadRequest(new MessageDto()
            //    {
            //        MessageAr = message.TypeNotExistsAr,
            //        MessageEn = message.TypeNotExistsEn,
            //    });
            //}
            result.Link = request.Link;
            if (result != null)
            {
                result.UpdatedAt = DateTime.Now;
                _externalLink.Update(result);
            }
            else return BadRequest(new MessageDto
            {
                MessageAr = message.FailedAr,
                MessageEn = message.FailedEn,
            });
            if (!await _externalLink.SaveChangesAsync())
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.FailedAr,
                    MessageEn = message.FailedEn,
                });
            }
            return Ok();
        }

        [Authorize(Roles = "Adminstrator"), HttpGet("GetAllLinks")]
        public async Task<IActionResult> GetAllLinks(int page=1,string query="")
        {
            var result =  query == null ? await _externalLink.GetAllLinksAsync() : await _externalLink.SearchByName(query);
            if (result != null && result.Any())
            {
                var list = PagedList<GetAllLinksResponseDto>.ToPagedList(result, page, 10);
                return Ok(new { Data = list, Pagination = list.Paganation });
            }

            return Ok(new MessageDto
            {
                MessageAr = message.NotFoundAr,
                MessageEn = message.NotFoundEn,
            }); ;

        }

        [AllowAnonymous, HttpGet("GetLinkForHomepage")]
        public async Task<IActionResult> GetLinkForHomepage()
        {
            var result = await _externalLink.GetByAllLinksAsync();
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}
