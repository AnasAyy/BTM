using BTMBackend.Dtos.CategoryDto;
using BTMBackend.Dtos.Product;
using BTMBackend.Models;
using BTMBackend.Utilities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using System.Collections.Generic;
using static Azure.Core.HttpHeader;
using static System.Net.Mime.MediaTypeNames;

namespace BTMBackend.Data.Repos
{
    public interface IProductRepo
    {
        public Task CreateAsync(Product product);
        public Task CreateAccessoriesFeatures(List<Accessories_Features> list);
        public void UpdateAccessoriesFeatures(Accessories_Features accessoriesFeature);
        public Task<bool> GetProductByNameForAddAsync(string nameAR, string nameEN);
        public void Update(Product product);
        public Task<bool> GetProductByNameForUpdateAsync(string nameAR, string nameEN, int id);
        public Task<Product?> GetByIdAsync(int id);
        public Task<IQueryable<GetAllProductResponceDto>> GetAllAsync();
        public Task<IQueryable<GetAllProductResponceDto>> SearchByNameAsync(string Query);
        public Task<GetProductDetailsResponceDto> GetProductDetails(int id);
        public void DeleteAccessoriesFeatures(Accessories_Features accessories_Features);
        public Task<Accessories_Features?> GetAccessories_FeaturesByIdAsync(int id);
        public Task<List<AccessoriesFeaturesResponseDto>> GetProductDetailsAccessories(int id);
        public Task<List<AccessoriesFeaturesResponseDto>> GetProductDetailsFeatures(int id);
        public Task<IQueryable<GetCategoryResponceDto>> GetAllCategoryAsync();
        public Task<List<GetProductPartsResopnseDto>> GetParts(int id);
        public Task<bool> SaveChangesAsync();
    }

    public class ProductRepo(DataContext context, UploadFileService uploadFileService) : IProductRepo
    {
        private readonly DataContext _context = context;
        private readonly UploadFileService _uploadFileService = uploadFileService;
        public async Task CreateAccessoriesFeatures(List<Accessories_Features> list)
        {
            foreach (var item in list)
            {
                await _context.accessories_Features.AddAsync(item);
            }
        }
        public void UpdateAccessoriesFeatures(Accessories_Features accessoriesFeature)
        {

            try
            {
                _context.accessories_Features.Update(accessoriesFeature);
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                Console.WriteLine(ex.Message);
            }
        }
        public async Task CreateAsync(Product product)
        {
                await _context.Products.AddAsync(product);
        }
        public async Task<IQueryable<GetAllProductResponceDto>> GetAllAsync()
        {
            var query = from p in _context.Products
                        join c in _context.Categories on p.CategoryId equals c.Id
                        orderby p.Id descending
                        select new GetAllProductResponceDto
                        {
                            Id = p.Id,
                            NameAr = p.NameAr,
                            NameEn = p.NameEn,
                            Status = p.IsActive,
                            Price = p.Price,
                            HasOffer = p.HasOffer,
                            CategoryNameAr = c.NameAr,
                            CategoryNameEn = c.NameEn
                        };

            return await Task.FromResult(query);
        }
        public async Task<IQueryable<GetAllProductResponceDto>> SearchByNameAsync(string Query)
        {
            var query = from p in _context.Products
                        join c in _context.Categories on p.CategoryId equals c.Id
                        where p.NameAr.Contains(Query) || p.NameEn.Contains(Query)
                        orderby p.Id descending
                        select new GetAllProductResponceDto
                        {
                            Id = p.Id,
                            NameAr = p.NameAr,
                            NameEn = p.NameEn,
                            Status = p.IsActive,
                            Price = p.Price,
                            HasOffer = p.HasOffer,
                            CategoryNameAr = c.NameAr,
                            CategoryNameEn = c.NameEn
                        };

            return await Task.FromResult(query);
        }
        public async Task<Product?> GetByIdAsync(int id)
        {
            var x = await _context.Products.SingleOrDefaultAsync(x => x.Id == id);
            _context.ChangeTracker.Clear();
            return x;
        }
        public async Task<bool> GetProductByNameForAddAsync(string nameAR, string nameEN)
        {
            return await _context.Products.AnyAsync(x => x.NameAr == nameAR || x.NameEn == nameEN);
        }
        public async Task<bool> GetProductByNameForUpdateAsync(string nameAR, string nameEN, int id)
        {
            return await _context.Products.AnyAsync(x => (x.NameAr == nameAR || x.NameEn == nameEN) && x.Id != id);
        }
        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                Console.WriteLine(ex.Message);
            }

            return false;
        }
        public  void Update(Product product)
        {
            try
            {
                _context.Products.Update(product);
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                Console.WriteLine(ex.Message);
            }

        }
        public async Task<Accessories_Features?> GetAccessories_FeaturesByIdAsync(int id)
        {
            var x = await _context.accessories_Features.SingleOrDefaultAsync(x => x.Id == id);
            _context.ChangeTracker.Clear();
            return x;
        }
        public void DeleteAccessoriesFeatures(Accessories_Features accessories_Features)
        {
            try
            {
                    _context.accessories_Features.Remove(accessories_Features);
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                Console.WriteLine(ex.Message);
            }
        }
        public async Task<GetProductDetailsResponceDto> GetProductDetails(int id)
        {
            var imagePath = (from p in _context.Products
                             where p.Id == id
                             select p.ImageUrl).FirstOrDefault(); // Get the first ImageUrl or null
            byte[] photo = null!;

            if (imagePath != null)
            {
                photo = await _uploadFileService.ConvertFileToByteArrayAsync(imagePath);
            }

            var query = from p in _context.Products
                        join c in _context.Categories on p.CategoryId equals c.Id
                        join u in _context.Users on p.UserId equals u.Id
                        where p.Id == id
                        select new GetProductDetailsResponceDto
                        {
                            NameAr = p.NameAr,
                            NameEn = p.NameEn,
                            DescriptionAR = p.DescriptionAR,
                            DescriptionEN = p.DescriptionEN,
                            HasOffer = p.HasOffer,
                            Price = p.Price,
                            OfferPrice = p.OfferPrice,
                            WarrantyDuration = p.WarrantyDuration,
                            ManufacturingCountry = p.ManufacturingCountry,
                            Image = photo, 
                            Model = p.Model,
                            Brand = p.Brand,
                            IsActive = p.IsActive,
                            CreatedAt = p.CreatedAt,
                            UpdatedAt = p.UpdatedAt,
                            UserName = u.Username,
                            CategoryId = c.Id,
                            
                        };

            var result = await query.FirstOrDefaultAsync();
            if (result != null)
                return result;
                return null!; 
        }
        public async Task<List<AccessoriesFeaturesResponseDto>> GetProductDetailsAccessories(int id)
        {
            var accessories = await _context.accessories_Features
                .Where(a => a.ProductId == id && a.Type == 1)
                .Select(a => new AccessoriesFeaturesResponseDto
                {
                    Id= a.Id,
                    NameAr = a.NameAr,
                    NameEn = a.NameEn
                })
                .ToListAsync();

            return accessories;
        }
        public async Task<List<AccessoriesFeaturesResponseDto>> GetProductDetailsFeatures(int id)
        {
            var features = await _context.accessories_Features
                .Where(a => a.ProductId == id && a.Type == 2)
                .Select(a => new AccessoriesFeaturesResponseDto
                {
                    Id = a.Id,
                    NameAr = a.NameAr,
                    NameEn = a.NameEn
                })
                .ToListAsync();

            return features;
        }
        public async Task<List<GetProductPartsResopnseDto>> GetParts(int id)
        {
            var query = await(from p in _context.Parts
                        join
                        u in _context.Users on p.UserId equals u.Id
                        join pp in _context.Products on p.ProductId equals pp.Id
                        where pp.Id == id
                        select new GetProductPartsResopnseDto
                        {
                            Id=p.Id,
                            NameAr = p.NameAr,
                            NameEn = p.NameEn,
                            DescriptionAr = p.DescriptionAr,
                            DescriptionEn = p.DescriptionEn,
                            Status= p.Status,
                            ExpirationDate = p.ExpirationDate,
                            CreatedAt= p.CreatedAt,
                            UpdatedAt= p.UpdatedAt,
                            UserName=u.Username,

                        }).ToListAsync();

            return query!;
        }
        public async Task<IQueryable<GetCategoryResponceDto>> GetAllCategoryAsync()
        {
            var query = from c in _context.Categories
                        join u in _context.Users on c.UserId equals u.Id
                        join e in _context.Employees on u.Id equals e.UserId
                        where c.IsActive == true
                        select new GetCategoryResponceDto
                        {
                            Id = c.Id,
                            NameAr = c.NameAr,
                            NameEn = c.NameEn,
                        };

            return await Task.FromResult(query);
        }

    }
}
