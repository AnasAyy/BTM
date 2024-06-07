namespace BTMBackend.Utilities
{
    public class ImageConvert
    {

        static public byte[] ConvertToByte(IFormFile img)
        {
            using var memoryStream = new MemoryStream();
            img.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
    }
}
