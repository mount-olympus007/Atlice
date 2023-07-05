using Atlice.Domain.Abstract;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlice.Domain.Concrete
{
    public class BlobService:IBlobService
    {
        private IConfiguration Configuration;
        public BlobService(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public async Task<BlobContainerClient> GetBlobContainer(string? containerName = null)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString: Configuration["BlobService:atlice_AzureStorageConnectionString"]);
            BlobContainerClient? containerClient;
            if (!string.IsNullOrEmpty(containerName))
            {
                containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            }
            else
            {
                containerClient = blobServiceClient.GetBlobContainerClient("atliceapp");
            }
            await containerClient.CreateIfNotExistsAsync();
            return containerClient;
        }
    }
}
