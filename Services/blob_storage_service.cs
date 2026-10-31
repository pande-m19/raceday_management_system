using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;

namespace raceday_management_system.Services
{//start of namespace
    public class blob_storage_service
    {//start of class
        private readonly string _connectionString;
        private readonly string _containerName;

        public blob_storage_service(IConfiguration configuration)
        {
            _connectionString = configuration["AzureStorage:ConnectionString"] ?? throw new Exception("Azure Connection String missing");
            _containerName = configuration["AzureStorage:ContainerName"] ?? "raceday-images";
        }

        public async Task<string?> upload_file_async(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            //Create a client for the container
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            //Ensure the container exists (creates it if not)
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            //Generate a unique file name to prevent overwriting
            string fileExtension = Path.GetExtension(file.FileName);
            string fileName = $"{Guid.NewGuid()}{fileExtension}";

            //Get a reference to the blob
            var blobClient = containerClient.GetBlobClient(fileName);

            //Upload the file stream
            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
            }

            //Return the absolute URL of the uploaded image
            return blobClient.Uri.ToString();
        }

    }//end of class

}//end of namespace