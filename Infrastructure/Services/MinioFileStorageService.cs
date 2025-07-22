using Application.Interfaces;
using ErrorOr;
using Minio;
using Minio.DataModel.Args;

namespace Infrastructure.Services
{
    public class MinioFileStorageService : IFileStorageService
    {
        private readonly IMinioClient _minioClient;

        public MinioFileStorageService(IMinioClient minioClient)
        {
            _minioClient = minioClient;
        }

        public async Task<ErrorOr<bool>> UploadFileAsync(string bucketName, string objectName, string filePath, string contentType)
        {
            // Ensure the bucket exists
            bool found = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
            if (!found)
            {
                return Error.Validation("MinioFileStorageService.UploadFileAsync", $"The bucket with name {bucketName} does not exist.");
            }

            await _minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithFileName(filePath)
                .WithContentType(contentType));

            return true;
        }

        public async Task<ErrorOr<bool>> UploadFileAsync(string bucketName, string objectName, Stream fileStream, string contentType)
        {
            bool found = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
            if (!found)
            {
                return Error.Validation("MinioFileStorageService.UploadFileAsync", $"The bucket with name {bucketName} does not exist.");
            }

            await _minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithStreamData(fileStream)
                .WithObjectSize(fileStream.Length)
                .WithContentType(contentType));

            return true;
        }
    }
}
