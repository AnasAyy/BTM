using BTMBackend.Data.Repos;
using BTMBackend.Dtos.SMSGatewayDto;
using BTMBackend.Dtos.UserDto;
using BTMBackend.Models;
using BTMBackend.SyncDataServices.Http.SMGateway;
using BTMBackend.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BTMBackend.Controllers.SMSManagement
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SMSController(ISMSService sMSService , IConfiguration configuration , IUserRepo userRepo , IOTPMessageRepo oTPMessageRepo) : ControllerBase
    {
        private readonly ISMSService _sMSService = sMSService;
        private readonly IUserRepo _userRepo = userRepo;
        private readonly IOTPMessageRepo _otpMessageRepo = oTPMessageRepo;
        private readonly IConfiguration _configuration = configuration;
        private readonly Messages ms = new();

        [AllowAnonymous]
        [HttpPost("ReSendOTP")]
        public async Task<IActionResult> ReSendOTP(OTPRequestDto requestDto)
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
    }
}
