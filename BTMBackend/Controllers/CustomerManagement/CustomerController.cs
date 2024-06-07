using Azure.Core;
using BTMBackend.Data.Repos;
using BTMBackend.Dtos.CustomerDto;
using BTMBackend.Dtos.PublicListDto;
using BTMBackend.Models;
using BTMBackend.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace BTMBackend.Controllers.CustomerManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController(IUserRepo userRepo, ICustomerRepo customerRepo) : ControllerBase
    {
        private readonly IUserRepo _userRepo = userRepo;
        private readonly ICustomerRepo _customerRepo = customerRepo;
        private readonly Messages ms = new();

        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateCustomerRequestDto requestDto)
        {
            try
            {
                var checkUser = await _userRepo.IsUserExist(requestDto.PhoneNumber);
                if (checkUser)
                {
                    return BadRequest(new MessageDto()
                    {
                        MessageAr = ms.AlreadyExistAr,
                        MessageEn = ms.AlreadyExistEn,
                    });
                }

                var createUser = _userRepo.Create(new User()
                {
                    Username = requestDto.PhoneNumber,
                    Password = RandomCodeGenerator.GeneratePassword(),
                    RoleId = requestDto.RoleId,
                });
                if (!await _userRepo.SaveChanges())
                {
                    return BadRequest(new MessageDto()
                    {
                        MessageAr = ms.FailedAr,
                        MessageEn = ms.FailedEn
                    });
                }
                
                await _customerRepo.Create(new Customer()
                {
                    FirstName = requestDto.FirstName,
                    LastName = requestDto.LastName,
                    PhoneNumber = requestDto.PhoneNumber,
                    Address = requestDto.Address,
                    City = requestDto.City,
                    County = requestDto.County,
                    UserId = createUser.Id,
                });
                if (!await _customerRepo.SaveChanges())
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
                    MessageEn = ms.AddedSuccessfullyEn,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPut("Update")]
        public async Task<IActionResult> Update(UpdateCustomerRequestDto requestDto)
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


            var getCustomer = await _customerRepo.GetById(userId);
            if (getCustomer == null)
            {
                return NotFound(new MessageDto()
                {
                    MessageAr = ms.NotFoundAr,
                    MessageEn = ms.NotFoundEn,
                });
            }

            getCustomer.FirstName = requestDto.FirstName;
            getCustomer.LastName = requestDto.LastName;
            getCustomer.Email = requestDto.Email;
            getCustomer.Email = requestDto.Address;
            getCustomer.City = requestDto.City;
            getCustomer.County = requestDto.County;

            _customerRepo.Update(getCustomer);

            if (!await _customerRepo.SaveChanges())
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
                MessageEn = ms.UpdatedSuccessfullyEn,
            });
        }


        [HttpGet("GetAll")]
        public async Task<ActionResult<GetAllCustomersResponseDto>> GetAll(int page = 1, int row = 10)
        {
            var result = await _customerRepo.GetAll();
            
            var list = PagedList<GetAllCustomersResponseDto>.ToPagedList(result, page, row);
            Response.Headers.Append("X-Pagination", System.Text.Json.JsonSerializer.Serialize(list.Paganation));
            return Ok(list);
        }


        [HttpGet("GetCustomerDetails")]
        public async Task<ActionResult<GetCustomerDetailsResponseDto>> GetCustomerDetails()
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
            var result = await _customerRepo.GetCustomerDetailsById(userId);

            return Ok(result);
        }

    }
}
