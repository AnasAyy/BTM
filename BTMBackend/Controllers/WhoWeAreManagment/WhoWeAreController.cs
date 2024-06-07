using AutoMapper;
using BTMBackend.Data.Repos;
using BTMBackend.Dtos.AboutUsDto;
using BTMBackend.Dtos.WhoWeAreDto;
using BTMBackend.Models;
using BTMBackend.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BTMBackend.Controllers.WhoWeAreManagment
{
    [Route("api/[controller]")]
    [ApiController]
    public class WhoWeAreController(IWhoWeAreRepo whoWeAreRepo) : ControllerBase
    {
        private readonly IWhoWeAreRepo _whoWeAreRepo = whoWeAreRepo;
        readonly Messages message = new();

        [Authorize(Roles = "Adminstrator"), HttpPut("Update")]
        public async Task<IActionResult> Update(UpdateWhoWeAreRequestDto request)
        {


            if (request.Id.ToString() == null)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.EmptyValueAr,
                    MessageEn = message.EmptyValueEn
                });
            }



            var result = await _whoWeAreRepo.GetByIdAsync(request.Id);
            if (result == null) return NotFound();
            if (result != null)
            {
                result.DescriptionEn = request.DescriptionEn;
                result.DescriptionAr = request.DescriptionAr;
                result.UpdatedAt = DateTime.Now;
                if (result != null)
                    _whoWeAreRepo.Update(result);
            }
            else return BadRequest(new MessageDto
            {
                MessageAr = message.FailedAr,
                MessageEn = message.FailedEn,
            });
            if (!await _whoWeAreRepo.SaveChangesAsync())
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.FailedAr,
                    MessageEn = message.FailedEn,
                });
            }
            return Ok();
        }

        [Authorize(Roles = "Adminstrator"), HttpGet("Get")]
        public async Task<IActionResult> Get()
        {
            var result = await _whoWeAreRepo.Get();
            if (result != null)
            {
                return Ok(result);
            }

                return Ok(new MessageDto
            {
                MessageAr = message.NotFoundAr,
                MessageEn = message.NotFoundEn,
            });
        }
        
        [HttpGet("GetForHomePage")]
        public async Task<IActionResult> GetForHomePage()
        {
            var result = await _whoWeAreRepo.GetForHome();
            if (result != null)
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
