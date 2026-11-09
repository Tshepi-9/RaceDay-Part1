using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace RaceDay.Api.Services
{
    public class AzureBlobService
    {
        private readonly BlobContainerClient _containerClient;

        public AzureBlobService(IConfiguration configuration)
        {
            string connectionString =
                configuration["AzureBlobStorage:ConnectionString"]
                ?? throw new InvalidOperationException(
                    "Azure Blob Storage connection string is missing.");

            string containerName =
                configuration["AzureBlobStorage:ContainerName"]
                ?? "profile-pictures";

            var blobServiceClient =
                new BlobServiceClient(connectionString);

            _containerClient =
                blobServiceClient.GetBlobContainerClient(containerName);
        }

        public async Task<string> UploadProfilePictureAsync(
            Stream fileStream,
            string contentType,
            string fileExtension)
        {
            await _containerClient.CreateIfNotExistsAsync(
                PublicAccessType.None);

            string fileName = $"{Guid.NewGuid()}{fileExtension}";

            BlobClient blobClient =
                _containerClient.GetBlobClient(fileName);

            await blobClient.UploadAsync(
                fileStream,
                new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = contentType
                    }
                });

            return fileName;
        }

        public async Task<(Stream Content, string ContentType)>
            DownloadProfilePictureAsync(string fileName)
        {
            BlobClient blobClient =
                _containerClient.GetBlobClient(fileName);

            var response =
                await blobClient.DownloadStreamingAsync();

            string contentType =
                response.Value.Details.ContentType
                ?? "application/octet-stream";

            return (response.Value.Content, contentType);
        }
    }
}