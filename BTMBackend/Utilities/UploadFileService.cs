using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BTMBackend.Utilities
{
    public class UploadFileService(IWebHostEnvironment hostingEnvironment)
    {
        private readonly IWebHostEnvironment _hostingEnvironment = hostingEnvironment;

        public async Task<string> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Invalid file");
            }

            if (file.Length > 2 * 1024 * 1024) // Maximum file size of 2MB (2 * 1024 * 1024 bytes)
            {
                throw new ArgumentException("File size exceeds the maximum limit (2MB)");
            }

            string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string extension = Path.GetExtension(file.FileName).ToLower();
            if (!IsSupportedImageExtension(extension))
            {
                throw new ArgumentException("Unsupported file extension");
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return filePath;
        }

        public async Task<string> UploadPdfFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Invalid file");
            }

            if (file.Length > 10 * 1024 * 1024) // Maximum file size of 10MB (10 * 1024 * 1024 bytes)
            {
                throw new ArgumentException("File size exceeds the maximum limit (10MB)");
            }

            string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "PdfFiles");
            Directory.CreateDirectory(uploadsFolder);

            string extension = Path.GetExtension(file.FileName).ToLower();
            if (!IsSupportedPdfExtension(extension))
            {
                throw new ArgumentException("Unsupported file extension");
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 81920, useAsync: true))
            {
                await file.CopyToAsync(fileStream);
            }

            return filePath;
        }

        private static bool IsSupportedImageExtension(string extension)
        {
            string[] supportedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".svg"]; // Add more extensions if needed

            return supportedExtensions.Contains(extension);
        }

        private static bool IsSupportedPdfExtension(string extension)
        {
            string[] supportedExtensions = [".pdf"];

            return supportedExtensions.Contains(extension);
        }

        public async Task<byte[]> ConvertFileToByteArrayAsync(string filePath)
        {
            if (File.Exists(filePath))
            {
                using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
                byte[] fileBytes = new byte[fileStream.Length];
                await fileStream.ReadAsync(fileBytes.AsMemory(0, (int)fileStream.Length));
                return fileBytes;
            }

            return null!;
        }

        public byte[]? ConvertFileToByteArray(string filePath)
        {
            if (File.Exists(filePath))
            {
                using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
                byte[] fileBytes = new byte[fileStream.Length];
                _ = fileStream.ReadAsync(fileBytes.AsMemory(0, (int)fileStream.Length));
                return fileBytes;
            }

            return null;
        }

        public  async Task DeleteFileAsync(string filePath)
        {
            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
            }
            else
            {
                Console.WriteLine("File not found", filePath);
            }
        }
    }
}
