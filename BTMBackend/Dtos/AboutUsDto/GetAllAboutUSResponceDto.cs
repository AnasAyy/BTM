using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BTMBackend.Utilities;

namespace BTMBackend.Dtos.AboutUsDto
{
    public class GetAllAboutUSResponseDto
    {
        public int Id { get; set; }
        public string NameAr { get; set; } = null!;
        public string DescriptionAr { get; set; } = null!;
        public string NameEn { get; set; } = null!;
        public string DescriptionEn { get; set; } = null!;
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }


}