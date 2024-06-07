using AutoMapper;
using BTMBackend.Data.Repos;
using BTMBackend.Dtos.Product;
using BTMBackend.Models;
using BTMBackend.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BTMBackend.Controllers.ProductManagment
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProductRepo productRepo, IMapper mapper, UploadFileService uploadFileService) : ControllerBase
    {
        private readonly IProductRepo _productRepo = productRepo;
        private readonly IMapper _mapper = mapper;

        private readonly UploadFileService _uploadFileService = uploadFileService;
        private readonly Messages message = new();

        [Authorize(Roles = "Adminstrator"), HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] CreateProductRequestDto request)
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

            var productExists = await _productRepo.GetProductByNameForAddAsync(request.NameAr, request.NameEn);
            if (productExists)
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = message.AlreadyExistAr,
                    MessageEn = message.AlreadyExistEn,
                });
            }

            #endregion

            #region File Upload

            string filePath;
            try
            {
                filePath = await _uploadFileService.UploadFile(request.File);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            #endregion

            #region Create Product

            var product = _mapper.Map<Product>(request);
            product.ImageUrl = filePath;

            int.TryParse(userId, out int userIdValue);
            product.UserId = userIdValue;

            await _productRepo.CreateAsync(product);

            if (!await _productRepo.SaveChangesAsync())
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

        [Authorize(Roles = "Adminstrator"), HttpPost("CreateAccessoriesAndFeatures")]
        public async Task<IActionResult> CreateAccessoriesAndFeatures(AddAccessoriesAndFeatures request)
        {
            // Create Accessories list
            List<Accessories_Features> accessoriesList = new();
            for (int i = 0; i < request.Accessories.Count; i++)
            {
                Accessories_Features accessory = new()
                {

                    NameAr = request.Accessories[i].NameAr,
                    NameEn = request.Accessories[i].NameEn,
                    Type = 1, // Accessories status is 1
                    ProductId = request.ProductId
                };

                accessoriesList.Add(accessory);
            }
            await _productRepo.CreateAccessoriesFeatures(accessoriesList);

            // Create Features list
            List<Accessories_Features> featuresList = new();
            for (int i = 0; i < request.Features.Count; i++)
            {
                Accessories_Features feature = new()
                {
                    NameAr = request.Features[i].NameAr,
                    NameEn = request.Features[i].NameEn,
                    Type = 2, // Features status is 2
                    ProductId = request.ProductId
                };

                featuresList.Add(feature);
            }
            await _productRepo.CreateAccessoriesFeatures(featuresList);

            if (!await _productRepo.SaveChangesAsync())
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.FailedAr,
                    MessageEn = message.FailedEn,
                });
            }

            return Ok();
        }

        [Authorize(Roles = "Adminstrator"), HttpPut("Update")]
        public async Task<IActionResult> Update([FromForm] UpdateProductRequestDto request)
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

            var existingProduct = await _productRepo.GetByIdAsync(request.Id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            var isDuplicateProduct = await _productRepo.GetProductByNameForUpdateAsync(request.NameAr, request.NameEn, request.Id);
            if (isDuplicateProduct)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.AlreadyExistAr,
                    MessageEn = message.AlreadyExistEn
                });
            }

            #endregion

            #region File Upload
            string filePath = string.Empty;
            if (request.File != null)
            {

                try
                {
                    filePath = await _uploadFileService.UploadFile(request.File);
                    await _uploadFileService.DeleteFileAsync(existingProduct.ImageUrl);
                    existingProduct.ImageUrl = filePath;
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            #endregion

            #region Update Product

            existingProduct.UpdatedAt = DateTime.Now;

            existingProduct.ImageUrl = filePath;
            _mapper.Map(request, existingProduct);

            _productRepo.Update(existingProduct);
            if (!await _productRepo.SaveChangesAsync())
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
        
        
        [Authorize(Roles = "Adminstrator"), HttpPut("UpdateStatus")]
        public async Task<IActionResult> Update(int id,bool isActive)
        {
            #region Validation

            if (id.ToString() == string.Empty && string.IsNullOrEmpty(isActive.ToString()))
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.EmptyValueAr,
                    MessageEn = message.EmptyValueEn
                });
            }

            var existingProduct = await _productRepo.GetByIdAsync(id);
            if (existingProduct == null)
            {
                return NotFound();
            }
            #endregion

            

            #region Update Product

            existingProduct.IsActive = isActive;

            _productRepo.Update(existingProduct);
            if (!await _productRepo.SaveChangesAsync())
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

        [Authorize(Roles = "Adminstrator"), HttpPut("UpdateAccessoriesAndFeatures")]
        public async Task<IActionResult> UpdateAccessoriesAndFeatures(UpdateAccessoriesAndFeatures request)
        {
            if (request.Id.ToString() == string.Empty)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.EmptyValueAr,
                    MessageEn = message.EmptyValueEn
                });
            }

            var existing = await _productRepo.GetAccessories_FeaturesByIdAsync(request.Id);
            if (existing == null)
            {
                return NotFound();
            }


            existing.NameEn = request.NameEn;
            existing.NameAr = request.NameAr;

            _productRepo.UpdateAccessoriesFeatures(existing);
            if (!await _productRepo.SaveChangesAsync())
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.FailedAr,
                    MessageEn = message.FailedEn,
                });
            }
            return Ok();
        }

        [Authorize(Roles = "Adminstrator"), HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(int page=1,string query="")
        {
            var result = query == null ?  await _productRepo.GetAllAsync(): await _productRepo.SearchByNameAsync(query);
            if (result != null && result.Any())
            {
                var list = PagedList<GetAllProductResponceDto>.ToPagedList(result, page, 10);
                return Ok(new { Data = list, Pagination = list.Paganation });
            }
            return Ok(new MessageDto
            {
                MessageAr = message.NotFoundAr,
                MessageEn = message.NotFoundEn,
            }); ;

        }

        [Authorize(Roles = "Adminstrator"), HttpDelete("DeleteAccessoriesFeature")]
        public async Task<IActionResult> DeleteAccessoriesFeature(DeleteAccessoriesFeatureRequestDto request)
        {
            if(string.IsNullOrEmpty(request.Id.ToString()))
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.EmptyValueAr,
                    MessageEn = message.EmptyValueEn
                });
            }
            var accessoriesFeature = await _productRepo.GetAccessories_FeaturesByIdAsync(request.Id);

            if (accessoriesFeature == null)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.FailedAr,
                    MessageEn = message.FailedEn,
                });
            }
            _productRepo.DeleteAccessoriesFeatures(accessoriesFeature);

            if (!await _productRepo.SaveChangesAsync())
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.FailedAr,
                    MessageEn = message.FailedEn,
                });
            }
            return Ok();
        }

        [Authorize(Roles = "Adminstrator"), HttpGet("GetDetails")]
        public async Task<IActionResult> GetDetails(int id)
        {
            if (string.IsNullOrEmpty(id.ToString()))
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.EmptyValueAr,
                    MessageEn = message.EmptyValueEn
                });
            }

            var result = await _productRepo.GetProductDetails(id);
            

            if (result == null)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.NotFoundAr,
                    MessageEn = message.NotFoundEn,
                });
            }

            return Ok(new
            {
                Result = result,
               });
        }

        [Authorize(Roles = "Adminstrator"), HttpGet("GetAccAndFea")]
        public async Task<IActionResult> GetAccAndFea(int id)
        {
            if (string.IsNullOrEmpty(id.ToString()))
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.EmptyValueAr,
                    MessageEn = message.EmptyValueEn
                });
            }

            var accessory = await _productRepo.GetProductDetailsAccessories(id);
            var feature = await _productRepo.GetProductDetailsFeatures(id);

            if (accessory == null && feature == null)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.NotFoundAr,
                    MessageEn = message.NotFoundEn,
                });
            }

            return Ok(new
            {
                Accessory = accessory,
                feature = feature,
            });
        }
        [Authorize(Roles = "Adminstrator"), HttpGet("GetParts")]
        public async Task<IActionResult> GetParts(int id)
        {
            if (string.IsNullOrEmpty(id.ToString()))
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.EmptyValueAr,
                    MessageEn = message.EmptyValueEn
                });
            }

            var result = await _productRepo.GetParts(id);

            if (result == null)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = message.NotFoundAr,
                    MessageEn = message.NotFoundEn,
                });
            }

            return Ok(result);
        }

        [Authorize(Roles = "Adminstrator"), HttpGet("GetAllCategory")]
        public async Task<IActionResult> GetAllCounties()
        {
            var result = await _productRepo.GetAllCategoryAsync();
            if (result != null && result.Any())
            {
                var list = result.Select(r => new GetCategoryResponceDto
                {
                    Id = r.Id,
                    NameAr = r.NameAr,
                    NameEn = r.NameEn,
                }).ToList();
                return Ok(list);
            }
            return NotFound();
        }

    }
}

