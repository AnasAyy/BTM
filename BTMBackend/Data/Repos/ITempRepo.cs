using BTMBackend.Dtos.Product;
using BTMBackend.Dtos.PublicListDto;
using BTMBackend.Models;
using BTMBackend.Utilities;
using Microsoft.EntityFrameworkCore;

namespace BTMBackend.Data.Repos
{
    public interface ITempRepo
    {
        public Task<IQueryable<GetAllContentProductResponseDto>> GetAll();
        public Task<IQueryable<GetAllContentProductResponseDto>> GetTop5();
        public Task<IQueryable<AppGetAllProductResponseDto>> AppGetAll();
        public Task<IQueryable<GetAllContentProductResponseDto>> AppGetAllByName(string productName);
        public Task<IQueryable<GetAllContentProductResponseDto>> AppGetFiltered(int categoryId, decimal fromPrive, decimal toPrice);
        public Task<IQueryable<GetPublicListResponseDto>> GetAllServiceTypes();
        public Task<IQueryable<GetProductDetailsResponseDto>> GetProductFeaturesById(int productId);
        public Task<IQueryable<GetProductDetailsResponseDto>> GetProductAccessoriesById(int productId);
        public Task<IQueryable<GetProductPartsResponseDto>> GetProductPartsById(int productId);
        public Task<IQueryable<GetAllContentProductResponseDto>> GetProductDetails(int id);
    }
    public class TempRepo(DataContext context, UploadFileService fileService) : ITempRepo
    {
        private readonly DataContext _context = context;
        private readonly UploadFileService _fileService = fileService;

        public async Task<IQueryable<AppGetAllProductResponseDto>> AppGetAll()
        {
            var result = await _context.Products.Select(x => new AppGetAllProductResponseDto()
            {
                ProductId = x.Id,
                Image = x.ImageUrl != null ? _fileService.ConvertFileToByteArray(x.ImageUrl) : null,  // Assuming ConvertFileToByteArrayAsync is the async version
                NameAr = x.NameAr,
                NameEn = x.NameEn,
                Price = x.Price,
                HasOffer = x.HasOffer,
                OfferPrice = x.OfferPrice,
                WarrantyDuration = x.WarrantyDuration
            }).ToListAsync();

            return result.AsQueryable();
        }
        public async Task<IQueryable<GetAllContentProductResponseDto>> AppGetAllByName(string productName)
        {
            var result = await _context.Products
                .Where(x => EF.Functions.Like(x.NameAr, $"%{productName}%") || EF.Functions.Like(x.NameEn, $"%{productName}%"))
                .Select(x => new GetAllContentProductResponseDto()
                {
                    ProductId = x.Id,
                    Image = x.ImageUrl != null ? _fileService.ConvertFileToByteArray(x.ImageUrl) : null,  // Assuming ConvertFileToByteArrayAsync is the async version
                    NameAr = x.NameAr,
                    NameEn = x.NameEn,
                    Price = x.Price,
                    HasOffer = x.HasOffer,
                    OfferPrice = x.OfferPrice,
                    WarrantyDuration = x.WarrantyDuration
                }).ToListAsync();

            return result.AsQueryable();
        }

        public async Task<IQueryable<GetAllContentProductResponseDto>> GetAll()
        {
            var result = await _context.Products.Select(x => new GetAllContentProductResponseDto()
            {
                ProductId = x.Id,
                Image = x.ImageUrl != null ? _fileService.ConvertFileToByteArray(x.ImageUrl) : null,  // Assuming ConvertFileToByteArrayAsync is the async version
                NameAr = x.NameAr,
                NameEn = x.NameEn,
                Model = x.Model,
                Brand = x.Brand,
                DescriptionAR = x.DescriptionAR,
                DescriptionEN = x.DescriptionEN,
                Price = x.Price,
                HasOffer = x.HasOffer,
                OfferPrice = x.OfferPrice,
                WarrantyDuration = x.WarrantyDuration
            }).ToListAsync();

            return result.AsQueryable();
        }


        public async Task<IQueryable<GetAllContentProductResponseDto>> GetTop5()
        {
            var result = await _context.Products
                .OrderByDescending(x => x.PurchaseTime) 
                .Take(5) 
                .Select(x => new GetAllContentProductResponseDto()
                {
                    ProductId = x.Id,
                    Image = x.ImageUrl != null ? _fileService.ConvertFileToByteArray(x.ImageUrl) : null,
                    NameAr = x.NameAr,
                    NameEn = x.NameEn,
                    Model = x.Model,
                    Brand = x.Brand,
                    DescriptionAR = x.DescriptionAR,
                    DescriptionEN = x.DescriptionEN,
                    Price = x.Price,
                    HasOffer = x.HasOffer,
                    OfferPrice = x.OfferPrice,
                    WarrantyDuration = x.WarrantyDuration
                })
                .ToListAsync();

            return result.AsQueryable();
        }


        public async Task<IQueryable<GetAllContentProductResponseDto>> GetProductDetails(int id)
        {
            var result = await _context.Products.Where(x => x.Id == id)
                .Select(x => new GetAllContentProductResponseDto()
                {
                    ProductId = x.Id,
                    Image = x.ImageUrl != null ? _fileService.ConvertFileToByteArray(x.ImageUrl) : null,
                    NameAr = x.NameAr,
                    NameEn = x.NameEn,
                    Model = x.Model,
                    Brand = x.Brand,
                    DescriptionAR = x.DescriptionAR,
                    DescriptionEN = x.DescriptionEN,
                    Price = x.Price,
                    HasOffer = x.HasOffer,
                    OfferPrice = x.OfferPrice,
                    WarrantyDuration = x.WarrantyDuration
                })
                .ToListAsync();

            return result.AsQueryable();
        }


        public async Task<IQueryable<GetPublicListResponseDto>> GetAllServiceTypes()
        {
            var result = await _context.PublicLists.Where(x => x.Code == "ServType" && x.Type != 0).Select(x => new GetPublicListResponseDto()
            {
                Id = x.Id,
                Code = x.Code,
                NameAR = x.NameAR,
                NameEN = x.NameEN,

            }).ToListAsync();

            return result.AsQueryable();
        }

        public async Task<IQueryable<GetProductDetailsResponseDto>> GetProductFeaturesById(int productId)
        {
            var result = await _context.accessories_Features.Where(x => x.ProductId == productId && x.Type == 2).Select(x => new GetProductDetailsResponseDto()
            {
                Id = x.Id,
                ContentAr = x.NameAr,
                ContentEn = x.NameEn,

            }).ToListAsync();

            return result.AsQueryable();
        }

        public async Task<IQueryable<GetProductDetailsResponseDto>> GetProductAccessoriesById(int productId)
        {
            var result = await _context.accessories_Features.Where(x => x.ProductId == productId && x.Type == 1).Select(x => new GetProductDetailsResponseDto()
            {
                Id = x.Id,
                ContentAr = x.NameAr,
                ContentEn = x.NameEn,

            }).ToListAsync();

            return result.AsQueryable();
        }

        public async Task<IQueryable<GetProductPartsResponseDto>> GetProductPartsById(int productId)
        {
            var result = await _context.Parts.Where(x => x.ProductId == productId).Select(x => new GetProductPartsResponseDto()
            {
                Id = x.Id,
                NameAr = x.NameAr,
                NameEn = x.NameEn,
                DescriptionAr = x.DescriptionAr,
                DescriptionEn = x.DescriptionEn,

            }).ToListAsync();

            return result.AsQueryable();
        }

        public async Task<IQueryable<GetAllContentProductResponseDto>> AppGetFiltered(int categoryId, decimal fromPrice, decimal toPrice)
        {
            IQueryable<Product> query = _context.Products;

            if (categoryId > 0 && fromPrice > 0 && toPrice > 0)
            {
                query = query.Where(x => x.CategoryId == categoryId && x.Price >= fromPrice && x.Price <= toPrice);
            }
            else if (categoryId > 0 && fromPrice <= 0 && toPrice <= 0)
            {
                query = query.Where(x => x.CategoryId == categoryId);
            }
            else if (fromPrice > 0 && toPrice > 0)
            {
                query = query.Where(x => x.Price >= fromPrice && x.Price <= toPrice);
            }
            else
            {
                return Enumerable.Empty<GetAllContentProductResponseDto>().AsQueryable();
            }

            var result = await query
                .Select(x => new GetAllContentProductResponseDto()
                {
                    ProductId = x.Id,
                    Image = x.ImageUrl != null ? _fileService.ConvertFileToByteArray(x.ImageUrl) : null,
                    NameAr = x.NameAr,
                    NameEn = x.NameEn,
                    Price = x.Price,
                    HasOffer = x.HasOffer,
                    OfferPrice = x.OfferPrice,
                    WarrantyDuration = x.WarrantyDuration
                })
                .ToListAsync();

            return result.AsQueryable();
        }
    }
}
