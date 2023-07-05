using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlice.Domain.Abstract
{
    public interface IBlobService
    {
        Task<BlobContainerClient> GetBlobContainer(string? containerName = null);
    }
}
