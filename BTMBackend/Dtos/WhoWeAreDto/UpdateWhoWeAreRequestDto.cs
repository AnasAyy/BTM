namespace BTMBackend.Dtos.WhoWeAreDto
{
    public class UpdateWhoWeAreRequestDto
    {
        public int Id { get; set; }
        public string DescriptionAr { get; set; } = null!;
        public string DescriptionEn { get; set; } = null!;

    }
}
