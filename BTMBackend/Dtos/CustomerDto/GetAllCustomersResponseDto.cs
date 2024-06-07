using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Dtos.CustomerDto
{
    public class GetAllCustomersResponseDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string CountyAr { get; set; } = null!;
        public string CountyEn { get; set; } = null!;
        public string CityAr { get; set; } = null!;
        public string CityEn { get; set; } = null!;
        public string Address { get; set; } = null!;
        public int TotalPurchasesAmount { get; set; } = 0;

    }
}
