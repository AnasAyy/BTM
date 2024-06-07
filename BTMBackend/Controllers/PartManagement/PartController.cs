using AutoMapper;
using BTMBackend.Data.Repos;
using BTMBackend.Dtos.CategoryDto;
using BTMBackend.Dtos.PartDto;
using BTMBackend.Dtos.Product;
using BTMBackend.Models;
using BTMBackend.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BTMBackend.Controllers.PartManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartController(IPartRepo partRepo, IMapper mapper) : ControllerBase
    {
        private readonly IPartRepo _partRepo = partRepo;
        private readonly IMapper _mapper = mapper;
        readonly Messages message = new();

        [Authorize, HttpPost("Create")]
        public async Task<IActionResult> Create(CreatePartRequestDto request)
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

            var productExists = await _partRepo.GetPartByNameForAddAsync(request.NameAr, request.NameEn, request.ProductId);
            if (productExists)
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = message.AlreadyExistAr,
                    MessageEn = message.AlreadyExistEn,
                });
            }

            #endregion



            #region Create Product

            var part = _mapper.Map<Part>(request);
            int userIdValue;
            int.TryParse(userId, out userIdValue);
            part.UserId = userIdValue;
            await _partRepo.CreateAsync(part);
            if (!await _partRepo.SaveChangesAsync())
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

        [Authorize, HttpPut("Update")]
        public async Task<IActionResult> Update(UpdatePartRequestDto request)
        {
            #region Validation

            if (request.Id.ToString() == string.Empty)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.EmptyValueAr,
                    MessageEn = message.EmptyValueEn
                });
            }

            var existingPart = await _partRepo.GetByIdAsync(request.Id);
            if (existingPart == null)
            {
                return NotFound();
            }

            var isDuplicatePart = await _partRepo.GetPartByNameForUpdateAsync(request.NameAr, request.NameEn, request.Id);
            if (isDuplicatePart)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.AlreadyExistAr,
                    MessageEn = message.AlreadyExistEn
                });
            }

            #endregion

            #region Update Product

            existingPart.UpdatedAt = DateTime.Now;
            _mapper.Map(request, existingPart);

            _partRepo.Update(existingPart);

            if (!await _partRepo.SaveChangesAsync())
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
    }
}
