using BTMBackend.Data.Repos;
using BTMBackend.Dtos.NotificationDto;
using BTMBackend.Dtos.PublicDto;
using BTMBackend.Models;
using BTMBackend.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BTMBackend.Controllers.NotificationManagement
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController(INotificationRepo notificationRepo) : ControllerBase
    {
        private readonly INotificationRepo _notificationRepo = notificationRepo;
        private readonly Messages ms = new();

        [HttpGet("GetByUser")]
        public async Task<ActionResult<IEnumerable<GetNotificationResponseDto>>> GetNotifications()
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
            var notifications = await _notificationRepo.GetByUserId(userId);
            return Ok(notifications);
        }


        [HttpPut("Update")]
        public async Task<IActionResult> Update()
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

            var notifications = await _notificationRepo.ChangeNotificationStatus(userId);
            if(!notifications)
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.NoDataUpdatedAr,
                    MessageEn = ms.NoDataUpdatedEn
                });
            }

            return Ok(new MessageDto()
            {
                MessageAr = ms.UpdatedSuccessfullyAr,
                MessageEn = ms.UpdatedSuccessfullyEn
            });

        }


        [AllowAnonymous]
        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateNotificationRequestDto requestDto)
        {
            var notification = new Notification()
            {
                ContentAr = requestDto.ContentAr,
                ContentEn = requestDto.ContentEn,
                ReceiverId = requestDto.ReceiverId
            };
            var result = await _notificationRepo.CreateSingle(notification);
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
                MessageAr = ms.AddedSuccessfullyAr,
                MessageEn = ms.AddedSuccessfullyEn
            });
        }

    }
}
