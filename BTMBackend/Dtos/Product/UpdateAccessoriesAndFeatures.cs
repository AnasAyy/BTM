﻿using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Dtos.Product
{
    public class UpdateAccessoriesAndFeatures
    {

        public int Id { get; set; }
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
    }

    
}
