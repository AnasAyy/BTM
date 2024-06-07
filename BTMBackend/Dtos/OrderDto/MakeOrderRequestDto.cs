namespace BTMBackend.Dtos.OrderDto
{
    public class MakeOrderRequestDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public int CountyId { get; set; }
        public int CityId { get; set; }
        public string Address { get; set; } = null!;
        public int SericeTypeId { get; set; }
        public string Message { get; set; } = null!;
    }
}
