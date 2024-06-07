using AutoMapper;
using BTMBackend.Data.Repos;
using BTMBackend.Dtos.CategoryDto;
using BTMBackend.Dtos.ContactUsDto;
using BTMBackend.Dtos.PublicDto;
using BTMBackend.Models;
using BTMBackend.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BTMBackend.Controllers.ContactUsManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactUsController(IContactUsRepo contactUs, IMapper mapper) : ControllerBase
    {
        private readonly IContactUsRepo _contactUs = contactUs;
        private readonly IMapper _mapper = mapper;
        readonly Messages message = new();

        [AllowAnonymous, HttpGet("GetTypes")]
        public async Task<IActionResult> GetTypes()
        {
            var result = await _contactUs.GetTypeAsync();
            if (result != null && result.Count != 0)
            {
                return Ok(result);
            }
            return NotFound();
        }

        [AllowAnonymous, HttpPost("CreateContactMessage")]
        public async Task<IActionResult> CreateContactMessage(ContactUsRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Type.ToString()) || string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.PhoneNumber) || string.IsNullOrEmpty(request.Message))
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.EmptyValueAr,
                    MessageEn = message.EmptyValueEn
                });
            }

            if (!await _contactUs.CheckyTypeAsync(request.Type))
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = message.TypeNotExistsAr,
                    MessageEn = message.TypeNotExistsEn,
                });
            }

            var contactUs = _mapper.Map<ContactUs>(request);
            if (contactUs != null)
                await _contactUs.CreateAsync(contactUs);
            if (!await _contactUs.SaveChangesAsync())
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.FailedAr,
                    MessageEn = message.FailedEn,
                });
            }
            return Ok();
        }

        [Authorize(Roles = "Adminstrator"), HttpGet("GetContactUsMessage")]
        public async Task<IActionResult> GetContactUsMessage(int page=1, string query="")
        {
            
            var result = query == null ? await _contactUs.GetAllMessage() : await _contactUs.SearchByName(query);

            if (result != null && result.Any())
            {

                var list = PagedList<GetAllMessagesResponseDto>.ToPagedList(result, page, 10);
                return Ok(new { Data = list, Pagination = list.Paganation });

            }
            return Ok(new MessageDto
            {
                MessageAr = message.NotFoundAr,
                MessageEn = message.NotFoundEn,
            }); ;

        }
    }
}
