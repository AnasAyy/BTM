using AutoMapper.Features;
using Azure.Core;
using BTMBackend.Data;
using BTMBackend.Data.Repos;
using BTMBackend.Dtos.AboutUsDto;
using BTMBackend.Dtos.Product;
using BTMBackend.Dtos.PublicListDto;
using BTMBackend.Dtos.SMSGatewayDto;
using BTMBackend.SyncDataServices.Http.SMGateway;
using BTMBackend.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BTMBackend.Controllers.TempController
{
    [Route("api/[controller]")]
    [ApiController]
    public class TempController(ITempRepo tempRepo , IConfiguration configuration, ISMSService sMSService) : ControllerBase
    {
        private readonly ITempRepo _tempRepo = tempRepo;
        private readonly ISMSService _smsService = sMSService;
        private readonly IConfiguration _configuration = configuration;
        private readonly Messages message = new();



        [HttpGet("AppGetAllProduct")]
        public async Task<ActionResult<List<GetAllContentProductResponseDto>>> AppGetAllProduct(int page = 1, int row = 10, string productName = "", int categoryId = 0, decimal fromPrice = 0, decimal toPrice = 0)
        {
            IQueryable<GetAllContentProductResponseDto> result;

            if (!string.IsNullOrEmpty(productName))
            {
                result = await _tempRepo.AppGetAllByName(productName);
            }
            else if (categoryId != 0 || (fromPrice > 0 && toPrice > 0))
            {
                result = await _tempRepo.AppGetFiltered(categoryId, fromPrice, toPrice);
            }
            else
            {
                result = await _tempRepo.GetAll();
            }

            var list = PagedList<GetAllContentProductResponseDto>.ToPagedList(result, page, row);
            Response.Headers.Append("X-Pagination", System.Text.Json.JsonSerializer.Serialize(list.Paganation));
            return Ok(list);
        }



        [HttpGet("GetAllServiceType")]
        public async Task<ActionResult<GetPublicListResponseDto>> GetAllServiceType()
        {
            var result = await _tempRepo.GetAllServiceTypes();
            return Ok(result);
        }


        [HttpGet("GetProductFeaturesById/{productId}")]
        public async Task<ActionResult<GetProductDetailsResponseDto>> GetProductFeaturesById(int productId)
        {
            var result = await _tempRepo.GetProductFeaturesById(productId);
            return Ok(result);
        }

        [HttpGet("GetProductAccessoriesById/{productId}")]
        public async Task<ActionResult<GetProductDetailsResponseDto>> GetProductAccessoriesById(int productId)
        {
            var result = await _tempRepo.GetProductAccessoriesById(productId);
            return Ok(result);
        }

        [HttpGet("GetProductPartsById/{productId}")]
        public async Task<ActionResult<GetProductPartsResponseDto>> GetProductPartsById(int productId)
        {
            var result = await _tempRepo.GetProductPartsById(productId);
            return Ok(result);
        }


        [HttpPost("TestSMS")]   
        public async Task<ActionResult<SMSResponseDto>> TestSMS(SendMessageRequestDto requestDto)
        {
            var result = await _smsService.SendSMS(requestDto);
            return Ok(result);
        }

        [HttpPost("TestSMSMessage")]
        public async Task<IActionResult> TestMessage()
        {
            var apiKey = _configuration["SMSAPIKey"];
            if(apiKey == null)
            {
                return Forbid();
            }
            var getOtp = RandomCodeGenerator.GenerateOTP();
            if(getOtp == null)
            {
                return BadRequest();
            }

            var getMessage = SMSMessages.SendOTP(getOtp);
            if (getMessage == null)
            {
                return BadRequest();
            }
            List<string> listofData = [];
            listofData.Add("966556229400");

            var data = new SendRequestDto()
            {
                Source = "BRONZE NET",
                Message = getMessage,
                Destination = listofData
            };

            var result = await _smsService.SendMessage(data , apiKey);
            if (!result)
            {
                return BadRequest();
            }

            return Ok();
        }



        [HttpGet("WebsiteGetAllProduct")]
        public async Task<ActionResult<List<GetAllContentProductResponseDto>>> WebsiteGetAllProduct(int page = 1, int row = 10, string productName = "", int categoryId = 0, decimal fromPrice = 0, decimal toPrice = 0)
        {
            IQueryable<GetAllContentProductResponseDto> result;

            if (!string.IsNullOrEmpty(productName))
            {
                result = await _tempRepo.AppGetAllByName(productName);
            }
            else if (categoryId != 0 || (fromPrice > 0 && toPrice > 0))
            {
                result = await _tempRepo.AppGetFiltered(categoryId, fromPrice, toPrice);
            }
            else
            {
                result = await _tempRepo.GetAll();
            }
            var list = PagedList<GetAllContentProductResponseDto>.ToPagedList(result, page, row);
            return Ok(new { Data = list, Pagination = list.Paganation });
        }
        
        [HttpGet("WebsiteHomePageGetAllProduct")]
        public async Task<ActionResult<List<GetAllContentProductResponseDto>>> WebsiteHomePageGetAllProduct()
        {
            IQueryable<GetAllContentProductResponseDto> result;
            result = await _tempRepo.GetTop5();
            if (result == null)
            {
                return Ok(new MessageDto
                {
                    MessageAr = message.EmptyValueAr,
                    MessageEn = message.EmptyValueEn
                });
            }
            return Ok(result);
        }

        [HttpGet("GetProductDetailesById/{productId}")]
        public async Task<ActionResult<GetProductDetailsResponseDto>> GetProductDetailesById(int productId)
        {
            var product = await _tempRepo.GetProductDetails(productId);
            var accessories = await _tempRepo.GetProductAccessoriesById(productId);
            var features = await _tempRepo.GetProductFeaturesById(productId);
            var parts = await _tempRepo.GetProductPartsById(productId);
            return Ok( new {
                Product = product,
                Accessories = accessories,
                Features = features,
                Parts = parts,
            });
        }
    }
}
