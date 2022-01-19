namespace MovieProMVC.Services.Interfaces
{
    public interface IImageService
    {
        Task<byte[]> EncodeImageAsync(IFormFile poster);
        Task<byte[]> EncodeImageAsync(string fileName);
        Task<byte[]> EncodeImageUrlAsync(string imageUrl);
        string DecodeImage(byte[] poster, string contentType);
        string ContentType(IFormFile file);
    }
}
