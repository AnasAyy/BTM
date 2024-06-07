using AutoMapper;
using Azure.Core;
using BTMBackend.Data.Repos;
using BTMBackend.Dtos.CategoryDto;
using BTMBackend.Dtos.PublicDto;
using BTMBackend.Dtos.PublicListDto;
using BTMBackend.Models;
using BTMBackend.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BTMBackend.Controllers.PublicListManagement
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Adminstrator")]
    public class PublicListController : ControllerBase
    {
        private readonly IPublicListRepo _publicList;
        private readonly IMapper _mapper;
        readonly Messages message = new();

        public PublicListController(IPublicListRepo publicList, IMapper mapper)
        {
            _publicList = publicList;
            _mapper = mapper;
        }

        [Authorize(Roles = "Adminstrator"), HttpPost("AddItem")]
        public async Task<IActionResult> AddItem(AddItemRequestDto request)
        {
            if (string.IsNullOrEmpty(request.NameEN) || string.IsNullOrEmpty(request.NameAR))
            {
                return BadRequest(new MessageDto { MessageAr = message.EmptyValueAr, MessageEn = message.EmptyValueEn });

            }
            var item = await _publicList.GetItemByNameForAddAsync(request.NameAR, request.NameEN);
            if (item)
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = message.AlreadyExistAr,
                    MessageEn = message.AlreadyExistEn,
                });
            }
            if (!await _publicList.CheckyTypeAsync(request.Code))
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = message.TypeNotExistsAr,
                    MessageEn = message.TypeNotExistsEn,
                });
            }
            var PL = _mapper.Map<PublicList>(request);
            if (PL != null)
            {
                await _publicList.CreateAsync(PL);
            }
            if (!await _publicList.SaveChangesAsync())
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.FailedAr,
                    MessageEn = message.FailedEn,
                });
            }
            return Ok();
        }

        [Authorize(Roles = "Adminstrator"), HttpPut("UpdateItem")]
        public async Task<IActionResult> UpdateItem(UpdateItemRequestDto request)
        {
            if (string.IsNullOrEmpty(request.NameEN) || string.IsNullOrEmpty(request.NameAR))
            {
                return BadRequest(new MessageDto { MessageAr = message.EmptyValueAr, MessageEn = message.EmptyValueEn });
            }

            var result = await _publicList.GetByIdAsync(request.Id);
            if (result == null) return NotFound();

            //var existingItem = await _publicList.GetItemByNameForUpdateAsync(request.NameAR, request.NameEN, request.Type, request.Id);
            //if (existingItem)
            //{
            //    return BadRequest(new MessageDto { MessageAr = message.AlreadyExistAr, MessageEn = message.AlreadyExistEn });
            //}

            //if (!await _publicList.CheckyTypeAsync(request.Type))
            //{
            //    return BadRequest(new MessageDto()
            //    {
            //        MessageAr = message.TypeNotExistsAr,
            //        MessageEn = message.TypeNotExistsEn,
            //    });
            //}
            result.Code = request.Code;
            result.NameAR = request.NameAR;
            result.NameEN = request.NameEN;
            result.Status = request.Status;
            result.UpdatedAt = DateTime.Now;

            _publicList.Update(result);


            if (!await _publicList.SaveChangesAsync())
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.FailedAr,
                    MessageEn = message.FailedEn,
                });
            }
            return Ok();
        }

        [HttpGet("GetAllItems")]
        public async Task<IActionResult> GetAllItems(int page = 1, string code = "", string query = null!)
        {


            var result = query == null ? await _publicList.GetAllAsync(code) : await _publicList.SearchByName(code, query);
            if (result != null && result.Any())
            {
                var list = PagedList<GetAllItemsResponseDto>.ToPagedList(result, page, 10);
                return Ok(new { Data = list, Pagination = list.Paganation });
            }

            return Ok(new MessageDto
            {
                MessageAr = message.NotFoundAr,
                MessageEn = message.NotFoundEn,
            }); ;
        }

        [Authorize(Roles = "Adminstrator"), HttpGet("GetAllMainItems")]
        public async Task<IActionResult> GetAllMainItems(int page = 1, string query = "")
        {

            var result = query == null ? await _publicList.GetMainItemAsync() : await _publicList.SearchMainByName(query);
            if (result != null && result.Any())
            {
                var list = PagedList<GetAllItemsResponseDto>.ToPagedList(result, page, 10);
                return Ok(new { Data = list, Pagination = list.Paganation });
            }

            return Ok(new MessageDto
            {
                MessageAr = message.NotFoundAr,
                MessageEn = message.NotFoundEn,
            }); ;



        }
        [Authorize(Roles = "Adminstrator"), HttpGet("GetAllMainItemsDL")]
        public async Task<IActionResult> GetAllMainItemsDL()
        {

            var result = await _publicList.GetMainItemAsync();
            if (result != null && result.Any())
            {
                return new JsonResult(new { result });
            }

            return Ok(new MessageDto
            {
                MessageAr = message.NotFoundAr,
                MessageEn = message.NotFoundEn,
            }); ;



        }


        [AllowAnonymous]
        [HttpGet("GetAllCounties")]
        public async Task<IActionResult> GetAllCounties()
        {
            var result = await _publicList.GetAllCounties();
            if (result != null && result.Any())
            {
                var list = result.Select(r => new GetPublicListResponseDto
                {
                    Id = r.Id,
                    NameAR = r.NameAR,
                    NameEN = r.NameEN,
                    Code = r.Code,
                }).ToList();
                return Ok(list);
            }
            return NotFound();
        }


        [AllowAnonymous]
        [HttpGet("GetItems")]
        public async Task<IActionResult> GetItems(GetItemRequestDto request)
        {
            var result = await _publicList.GetItems(request.Code);
            if (result != null && result.Any())
            {
                var list = result.Select(r => new GetPublicListResponseDto
                {
                    Id = r.Id,
                    NameAR = r.NameAR,
                    NameEN = r.NameEN,
                }).ToList();
                return Ok(list);
            }
            return NotFound();
        }

        [AllowAnonymous]
        [HttpGet("GetByCode/{code}")]
        public async Task<ActionResult<GetPublicListResponseDto>> GetByCode(string code)
        {
            var result = await _publicList.GetItems(code);

            var list = result.Select(r => new GetPublicListResponseDto
            {
                Id = r.Id,
                NameAR = r.NameAR,
                NameEN = r.NameEN,
            }).ToList();
            return Ok(list);

        }
    }
}
