using AutoMapper;
using BTMBackend.Data.Repos;
using BTMBackend.Dtos.StatisticDto;
using BTMBackend.Models;
using BTMBackend.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BTMBackend.Controllers.StatisticManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticController : ControllerBase
    {
        private readonly IStatisticRepo  _statisticRepo;
        private readonly IMapper _mapper;
        Messages message = new Messages();

        public StatisticController(IStatisticRepo statisticRepo, IMapper mapper)
        {
            _statisticRepo = statisticRepo;
            _mapper = mapper;
        }

        //[Authorize, HttpPost("AddStatistic")]
        //public async Task<IActionResult> AddStatistic(AddStatisticRequestDto request)
        //{
        //    if (request.Name == "" || request.Name == null || request.FakeValue.ToString() == "" || request.FakeValue.ToString() == null || request.UserId.ToString() == "" || request.UserId.ToString() == null)
        //    {
        //        return BadRequest(new MessageDto
        //        {
        //            MessageAr = message.EmptyValueAr,
        //            MessageEn = message.EmptyValueEn
        //        });

        //    }
        //    var name = await _statisticRepo.CheckName(request.Name);
        //    if (name)
        //    {
        //        return BadRequest(new MessageDto()
        //        {
        //            MessageAr = message.AlreadyExistAr,
        //            MessageEn = message.AlreadyExistEn,
        //        });
        //    }

        //    var S = _mapper.Map<Statistic>(request);
        //    if (S != null)
        //    {
        //        await _statisticRepo.CreateAsync(S);
        //    }
        //    if (!await _statisticRepo.SaveChangesAsync())
        //    {
        //        return BadRequest(new MessageDto
        //        {
        //            MessageAr = message.FailedAr,
        //            MessageEn = message.FailedEn,
        //        });
        //    }
        //    return Ok();
        //}

        [Authorize(Roles = "Adminstrator"), HttpPut("UpdateStatistic")]
        public async Task<IActionResult> UpdateStatistic(UpdateUpdateStatisticRequestDto request)
        {
            if (request.Id.ToString() == "" || request.Id.ToString() == null)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.EmptyValueAr,
                    MessageEn = message.EmptyValueEn
                });
            }

            var result = await _statisticRepo.GetByIdAsync(request.Id);
            if (result == null) return NotFound();

            //var existingItem = await _statisticRepo.GetItemByNameForUpdateAsync(request.Name, request.Id);
            //if (existingItem)
            //{
            //    return BadRequest(new MessageDto
            //    {
            //        MessageAr = message.AlreadyExistAr,
            //        MessageEn = message.AlreadyExistEn
            //    });
            //}

            //var S = _mapper.Map<Statistic>(request);
            result.FakeValue = request.FakeValue;
            result.Status=request.Status;
            if (result != null)
            {
                result.UpdatedAt = DateTime.Now;
                _statisticRepo.Update(result);
            }
            else return BadRequest(new MessageDto
            {
                MessageAr = message.FailedAr,
                MessageEn = message.FailedEn,
            });
            if (!await _statisticRepo.SaveChangesAsync())
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.FailedAr,
                    MessageEn = message.FailedEn,
                });
            }
            return Ok();
        }

        [HttpGet("GetAllStatistic")]
        public async Task<IActionResult> GetAllStatistic(int page = 1, string query = null!)
        {


            var result = query == null ? await _statisticRepo.GetAllAsync() : await _statisticRepo.SearchByName(query);
            if (result != null && result.Any())
            {
                var list = PagedList<GetAllStatisticResponseDto>.ToPagedList(result, page, 10);
                return Ok(new { Data = list, Pagination = list.Paganation });
            }
            else
                return Ok(new MessageDto
                {
                    MessageAr = message.NotFoundAr,
                    MessageEn = message.NotFoundEn,
                }); ;
        }
        
        
        [HttpGet("GetAllStatisticForHomePage")]
        public async Task<IActionResult> GetAllStatisticForHomePage()
        {


            var result =  await _statisticRepo.GetAllForHomePageAsync();
            if (result != null && result.Any())
            {
                return Ok(result);
            }
            else
                return Ok(new MessageDto
                {
                    MessageAr = message.NotFoundAr,
                    MessageEn = message.NotFoundEn,
                }); ;
        }
    }
}

