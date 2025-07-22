using ErrorOr;

namespace Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<ErrorOr<bool>> UploadFileAsync(string bucketName, string objectName, string filePath, string contentType);
        Task<ErrorOr<bool>> UploadFileAsync(string bucketName, string objectName, Stream fileStream, string contentType);
    }
}
