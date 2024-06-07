using BTMBackend.Data.Repos;
using BTMBackend.Dtos.AdSliderDto;
using BTMBackend.Dtos.EmployeeDto;
using BTMBackend.Models;
using BTMBackend.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BTMBackend.Controllers.EmployeeManagement
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeController(IEmployeeRepo employeeRepo, IUserRepo userRepo, IEncryptRepo encryptRepo) : ControllerBase
    {
        readonly Messages ms = new();
        private readonly IEmployeeRepo _employeeRepo = employeeRepo;
        private readonly IUserRepo _userRepo = userRepo;
        private readonly IEncryptRepo _encryptRepo = encryptRepo;

        [HttpPost("Create")]
        public async Task<ActionResult> Create(CreateEmployeeRequestDto requestDto)
        {
            if (await _userRepo.IsUserExist(requestDto.PhoneNumber))
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.AlreadyExistAr,
                    MessageEn = ms.AlreadyExistEn,
                });
            }

            var createUser = new User()
            {
                Username = requestDto.PhoneNumber,
                Password = _encryptRepo.EncryptPassword(requestDto.Password),
                IsActive = true,
                IsVerified = true,
                RoleId = requestDto.RoleId,
            };

            await _userRepo.Create(createUser);
            if (!await _userRepo.SaveChanges())
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.FailedAr,
                    MessageEn = ms.FailedEn,
                });
            }

            var createEmployee = new Employee()
            {
                FirstName = requestDto.FirstName,
                LastName = requestDto.LastName,
                PhoneNumber = requestDto.PhoneNumber,
                City = requestDto.City,
                County = requestDto.County,
                Address = requestDto.Address,
                UserId = createUser.Id,
            };

            await _employeeRepo.Create(createEmployee);

            if (!await _employeeRepo.SaveChanges())
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

        [Authorize, HttpGet("GetAll")]
        public async Task<ActionResult<GetAllEmployeeResponseDto>> GetAll(int page = 1, string query = "")
        {
            if (User.Identity is not ClaimsIdentity identity)
            {
                return Unauthorized();
            }

            var userIdClaim = identity.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var result = query == null ? await _employeeRepo.GetAll(userId) : await _employeeRepo.GetByNameOrPhone(query , userId);
            var list = PagedList<GetAllEmployeeResponseDto>.ToPagedList(result, page, 10);

            return Ok(new { Data = list, Pagination = list.Paganation });
        }


        [HttpGet("GetEmployee")]
        public async Task<ActionResult<GetEmployeeforAppResponseDto>> GetEmployee()
        {
            if (User.Identity is not ClaimsIdentity identity)
            {
                return Unauthorized();
            }

            var userIdClaim = identity.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var employee = await _employeeRepo.GetByUserId(userId);

            if(employee == null)
            {
                return NotFound(new MessageDto()
                {
                    MessageAr = ms.NotFoundAr,
                    MessageEn = ms.NotFoundEn
                });
            }

            GetEmployeeforAppResponseDto result = new()
            {
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                PhoneNumber = employee.PhoneNumber,
                City = employee.City,
                County = employee.County,
                Address = employee.Address
            };

            return Ok(result);
        }


        [HttpGet("GetById/{employeeId}")]
        public async Task<ActionResult<GetEmployeeResponseDto>> GetById(int employeeId)
        {
            var result = await _employeeRepo.GetEmployeeById(employeeId);
            if (result == null)
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.NotFoundAr,
                    MessageEn = ms.NotFoundEn
                });
            }

            return Ok(result);
        }


        [HttpPut("Update")]
        public async Task<IActionResult> Update(UpdateEmployeeRequestDto requestDto)
        {
            if (User.Identity is not ClaimsIdentity identity)
            {
                return Unauthorized();
            }

            var userIdClaim = identity.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var result = await _employeeRepo.GetByUserId(userId);

            if (result == null)
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.NotFoundAr,
                    MessageEn = ms.NotFoundEn
                });
            }


            var user = await _userRepo.GetById(result.UserId);
            if (user == null)
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.NotFoundAr,
                    MessageEn = ms.NotFoundEn
                });
            }


            result.FirstName = requestDto.FirstName;
            result.LastName = requestDto.LastName;
            result.County = requestDto.County;
            result.City = requestDto.City;
            result.Address = requestDto.Address;
            result.UpdatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;

            _userRepo.Update(user);
            if (!await _userRepo.SaveChanges())
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.FailedToUpdateAr,
                    MessageEn = ms.FailedToUpdateEn
                });
            }

            _employeeRepo.Update(result);

            if (!await _employeeRepo.SaveChanges())
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.FailedToUpdateAr,
                    MessageEn = ms.FailedToUpdateEn
                });
            }

            return Ok(new MessageDto()
            {
                MessageAr = ms.UpdatedSuccessfullyAr,
                MessageEn = ms.UpdatedSuccessfullyEn
            });
        }



        [HttpPut("UpdateById")]
        public async Task<IActionResult> UpdateById(UpdateEmployeeByIdRequestDto requestDto)
        {
            var result = await _employeeRepo.GetByUserId(requestDto.Id);

            if (result == null)
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.NotFoundAr,
                    MessageEn = ms.NotFoundEn
                });
            }


            var user = await _userRepo.GetById(result.UserId);
            if (user == null)
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.NotFoundAr,
                    MessageEn = ms.NotFoundEn
                });
            }
            

            result.FirstName = requestDto.FirstName;
            result.LastName = requestDto.LastName;
            result.County = requestDto.County;
            result.City = requestDto.City;
            result.Address = requestDto.Address;
            result.UpdatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;
            user.IsActive = requestDto.IsAcyive;
            user.RoleId = requestDto.RoleId;

            _userRepo.Update(user);
            if (!await _userRepo.SaveChanges())
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.FailedToUpdateAr,
                    MessageEn = ms.FailedToUpdateEn
                });
            }

            _employeeRepo.Update(result);

            if (!await _employeeRepo.SaveChanges())
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.FailedToUpdateAr,
                    MessageEn = ms.FailedToUpdateEn
                });
            }

            return Ok(new MessageDto()
            {
                MessageAr = ms.UpdatedSuccessfullyAr,
                MessageEn = ms.UpdatedSuccessfullyEn
            });
        }



        [HttpGet("GetAllCallCenters")]
        public async Task<ActionResult<GetSpecificTypeOfEmployeeResponseDto>> GetAllCallCenters()
        {
            return Ok(await _employeeRepo.GetCallCenters());
        }



        [HttpGet("GetAllSupervisor")]
        public async Task<ActionResult<GetEmplyeesResponseDto>> GetAllSupervisor()
        {
            return Ok(await _employeeRepo.GetSupervisors());
        }



        [HttpGet("GetAllTechnician")]
        public async Task<ActionResult<GetEmplyeesResponseDto>> GetAllTechnician()
        {
            return Ok(await _employeeRepo.GetTechnician());
        }

    }
}
