using Azure.Core;
using BTMBackend.Data.Repos;
using BTMBackend.Dtos.AdSliderDto;
using BTMBackend.Dtos.PublicDto;
using BTMBackend.Dtos.SMSGatewayDto;
using BTMBackend.Dtos.UserDto;
using BTMBackend.Models;
using BTMBackend.SyncDataServices.Http.SMGateway;
using BTMBackend.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OtpNet;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BTMBackend.Controllers.UserManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserRepo userRepo, IRefreshTokenRepo refreshTokenRepo, IConfiguration configuration, IOTPMessageRepo oTPMessageRepo, ISMSService sMSService, IAuthorityRepo authorityRepo, IEncryptRepo encryptRepo) : ControllerBase
    {
        private readonly IUserRepo _userRepo = userRepo;
        private readonly IConfiguration _configuration = configuration;
        private readonly IEncryptRepo _encryptRepo = encryptRepo;
        private readonly IAuthorityRepo _authorityRepo = authorityRepo;
        private readonly ISMSService _sMSService = sMSService;
        private readonly IOTPMessageRepo _otpMessageRepo = oTPMessageRepo;
        private readonly IRefreshTokenRepo _refreshTokenRepo = refreshTokenRepo;

        readonly Messages ms = new();

        [AllowAnonymous]
        [HttpPost("SignIn")]
        public async Task<ActionResult<LogInResponseDto>> SignIn(LogInRrequestDto requestDto)
        {
            var encryptedPassword = _encryptRepo.EncryptPassword(requestDto.Password);
            if (string.IsNullOrEmpty(encryptedPassword))
            {
                return Unauthorized(new MessageDto
                {
                    MessageAr = ms.FailedLoginAr,
                    MessageEn = ms.FailedLoginEn,
                });
            }

            var getUser = await _userRepo.Login(requestDto.Username, encryptedPassword);
            if (getUser == null)
            {
                return Unauthorized(new MessageDto
                {
                    MessageAr = ms.FailedLoginAr,
                    MessageEn = ms.FailedLoginEn,
                });
            }

            var getRole = await _authorityRepo.GetRoleNameEnByUserId(getUser.Id);
            if (string.IsNullOrEmpty(getRole))
            {
                return Unauthorized(new MessageDto
                {
                    MessageAr = ms.UnAuthorizeAr,
                    MessageEn = ms.UnAuthorizeEn,
                });
            }

            var subject = _configuration["Jwt:Subject"];
            var keyHash = _configuration["Jwt:Key"];

            if (subject is null || keyHash is null)
            {
                return Unauthorized(new MessageDto
                {
                    MessageAr = ms.UnAuthorizeAr,
                    MessageEn = ms.UnAuthorizeEn,
                });
            }


            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                ExpiryDate = DateTime.UtcNow.AddDays(7), // Refresh tokens usually have a longer lifespan
                UserId = getUser.Id
            };

            var addRefreshToken = await _refreshTokenRepo.Create(refreshToken);
            if (!addRefreshToken)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = ms.FailedAr,
                    MessageEn = ms.FailedEn,
                });
            }

            var stringToken = GenerateAccessToken(new CreateTokenRequestDto()
            {
                UserId = getUser.Id,
                Role = getUser.PositionEn
            });

            if (stringToken == null)
            {
                return Unauthorized();
            }

            getUser.RefreshToken = refreshToken.Token;
            getUser.Token = stringToken;

            if (!await _userRepo.UpdateLastLoginDateById(getUser.Id))
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = ms.FailedAr,
                    MessageEn = ms.FailedEn,
                });
            }
            return Ok(getUser);
        }


        [AllowAnonymous]
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(GetOldRefreshTokenRequestDto requestDto)
        {
            var storedToken = await _refreshTokenRepo.GetByToken(requestDto.RefreshToken);

            if (storedToken == null || storedToken.ExpiryDate < DateTime.UtcNow)
            {
                return StatusCode(419);
            }

            var getUserData = await _userRepo.GetUserAndRole(storedToken.UserId);
            if (getUserData == null)
            {
                return StatusCode(419);
            }

            var newAccessToken = GenerateAccessToken(new CreateTokenRequestDto()
            {
                Role = getUserData.Role,
                UserId = getUserData.UserId
            });

            if (newAccessToken == null)
            {
                return StatusCode(419);
            }

            var deleteResult = await _refreshTokenRepo.Delete(storedToken);
            if (!deleteResult)
            {
                return StatusCode(419, new MessageDto()
                {
                    MessageAr = ms.FailedToDeleteAr,
                    MessageEn = ms.FailedToDeleteEn
                });
            }

            var newRefreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                UserId = getUserData.UserId
            };
            var addRefreshToken = await _refreshTokenRepo.Create(newRefreshToken);

            if (!addRefreshToken)
            {
                return StatusCode(419, new MessageDto()
                {
                    MessageAr = ms.FailedAr,
                    MessageEn = ms.FailedEn
                });
            }

            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            });
        }


        [Authorize]
        [HttpGet("IfVerified")]
        public async Task<IActionResult> IfVerified()
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

            var checkIfVerified = await _userRepo.IsUserVerified(userId);

            if (!checkIfVerified)
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.UserIsNotActiveAr,
                    MessageEn = ms.UserIsNotActiveEn
                });
            }

            return Ok(new MessageDto()
            {
                MessageAr = ms.UserIsActiveAr,
                MessageEn = ms.UserIsActiveEn
            });
        }


        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequestDto requestDto)
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

            if (requestDto.Password != requestDto.ConfirmPassword)
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.PasswordDontMatchAr,
                    MessageEn = ms.PasswordDontMatchEn
                });
            }

            var encryptedPassword = _encryptRepo.EncryptPassword(requestDto.Password);

            await _userRepo.UpdatePassword(encryptedPassword, userId);

            var saveChanges = await _userRepo.SaveChanges();

            if (!saveChanges)
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
                MessageEn = ms.UpdatedSuccessfullyEn
            });
        }

        [HttpPost("ChangeUserPassword")]
        public async Task<IActionResult> ChangeUserPassword(ChangeUserPasswordRequestDto requestDto)
        {
            if (requestDto.Password != requestDto.ConfirmPassword)
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.PasswordDontMatchAr,
                    MessageEn = ms.PasswordDontMatchEn
                });
            }

            var encryptedPassword = _encryptRepo.EncryptPassword(requestDto.Password);

            await _userRepo.UpdatePassword(encryptedPassword, requestDto.UserId);

            var saveChanges = await _userRepo.SaveChanges();

            if (!saveChanges)
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
                MessageEn = ms.PasswordDontMatchEn
            });
        }


        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(OTPRequestDto requestDto)
        {
            var getUser = await _userRepo.GetUserByPhoneNumber(requestDto.PhoneNumber);
            if (getUser == null)
            {
                return NotFound(new MessageDto()
                {
                    MessageAr = ms.PhoneNotFoundAr,
                    MessageEn = ms.PhoneNotFoundEn
                });
            }

            var otpCode = RandomCodeGenerator.GenerateOTP();
            if (otpCode == null)
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.FailedAr,
                    MessageEn = ms.FailedEn
                });
            }

            var getMessage = SMSMessages.SendOTP(otpCode);
            if (getMessage == null)
            {
                return BadRequest();
            }

            List<string> listofData = [];
            listofData.Add(requestDto.PhoneNumber);

            var messageData = new SendRequestDto()
            {
                Source = "BRONZE NET",
                Message = getMessage,
                Destination = listofData
            };

            var apiKey = _configuration["SMSAPIKey"];
            if (apiKey == null)
            {
                return Forbid();
            }



            await _sMSService.SendMessage(messageData, apiKey);

            DateTime currentTime = DateTime.Now;
            int minutesToAdd = 5;
            DateTime newTime = currentTime.AddMinutes(minutesToAdd);

            await _otpMessageRepo.DeleteOld();

            await _otpMessageRepo.Create(new OTPMessage()
            {
                Code = otpCode,
                ExpirationDatetime = newTime,
                PhoneNumber = requestDto.PhoneNumber,
                Status = 0,
            });

            var saveChages = await _otpMessageRepo.SaveChanges();

            if (!saveChages)
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.FailedAr,
                    MessageEn = ms.FailedEn
                });
            }


            return Ok(new MessageDto()
            {
                MessageAr = ms.SendSuccessfullyAr,
                MessageEn = ms.SendSuccessfullyEn
            });
        }


        [HttpPost("CheckOtp")]
        public async Task<ActionResult<ReturnTokenResponseDto>> CheckOtp(CheckOtpRequestDto requestDto)
        {
            var checkPhoneNumber = await _userRepo.IsUserExist(requestDto.PhoneNumber);
            if (!checkPhoneNumber)
            {
                return NotFound(new MessageDto()
                {
                    MessageAr = ms.PhoneNotFoundAr,
                    MessageEn = ms.PhoneNotFoundEn
                });
            }

            var checkOTP = await _otpMessageRepo.CheckIfValid(requestDto.OTPCode, requestDto.PhoneNumber);

            if (!checkOTP)
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.WrongOtpAr,
                    MessageEn = ms.WrongOtpEn
                });
            }
            await _otpMessageRepo.Delete(requestDto.OTPCode, requestDto.PhoneNumber);

            var saveChages = await _otpMessageRepo.SaveChanges();
            if (!saveChages)
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.FailedAr,
                    MessageEn = ms.FailedEn
                });
            }

            var getUser = await _userRepo.GetUserByPhoneNumber(requestDto.PhoneNumber);
            if (getUser == null)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = ms.FailedAr,
                    MessageEn = ms.FailedEn
                });
            }

            var getUserData = await _userRepo.GetUserAndRole(getUser.Id);
            if (getUserData == null)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = ms.FailedAr,
                    MessageEn = ms.FailedEn
                });
            }

            var stringToken = GenerateAccessToken(new CreateTokenRequestDto()
            {
                UserId = getUserData.UserId,
                Role = getUserData.Role
            });

            if (stringToken == null)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = ms.FailedAr,
                    MessageEn = ms.FailedEn
                });
            }

            return Ok(new ReturnTokenResponseDto()
            {
                AccessToken = stringToken,
            });
        }


        [Authorize]
        [HttpPut("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus(UpdateStatusRequestDto requestDto)
        {
            var user = await _userRepo.GetById(requestDto.Id);
            if (user == null)
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.NotFoundAr,
                    MessageEn = ms.NotFoundEn
                });
            }

            var result = await _userRepo.UpdateStatus(requestDto.Id);
            if (!result)
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
                MessageEn = ms.UpdatedSuccessfullyEn
            });
        }

        private string? GenerateAccessToken(CreateTokenRequestDto requestDto)
        {
            var subject = _configuration["Jwt:Subject"];
            var keyHash = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            if (subject == null || keyHash == null || issuer == null)
            {
                return null;
            }

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, subject),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new(ClaimTypes.NameIdentifier, requestDto.UserId.ToString() ?? string.Empty),
                new(ClaimTypes.Name, requestDto.UserId.ToString() ?? string.Empty),
                new(ClaimTypes.Role, requestDto.Role),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyHash));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expirationTime = DateTime.UtcNow.AddMinutes(10);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                Subject = new ClaimsIdentity(claims),
                Expires = expirationTime,
                SigningCredentials = signIn
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var stringToken = tokenHandler.WriteToken(token);

            return stringToken;
        }


    }
}
