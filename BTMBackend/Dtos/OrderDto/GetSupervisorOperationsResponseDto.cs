namespace BTMBackend.Dtos.OrderDto
{
    public class GetSupervisorOperationsResponseDto
    {
        public int Id { get; set; }
        public string CustomerFullName { get; set; } = null!;
        public string ServiceType { get; set; } = null!;
        public string CountyNameAr { get; set; } = null!;
        public string CountyNameEn { get; set; } = null!;
        public string CityNameAr {  get; set; } = null!;
        public string CityNameEn {  get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string StatusAr { get; set; } = null!;
        public string StatusEn { get; set; } = null!;
    }
}
