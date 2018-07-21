using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Xamarin;
using Xamarin.Forms;
using System.IO;

namespace ImCup2018
{
    class CloudStorage
    {
        public static class Config
        {
            public const string accountName = "ruslixag2";
            public const string accountKey = "IIh7AxJUpJ/ev+YreB1wcs4DWEHhZa7FWny16DadlW8EsTRsvb9cHno9Q7Ad1T0ACMLPd9GtStBZ5tYOIHoG5w==";
        }

        public static CloudBlobContainer GetBlobContainer(string containerName)
        {
            string connectionString = $"DefaultEndpointsProtocol=https;" +
                $"AccountName={Config.accountName};" +
                $"AccountKey={Config.accountKey}";

            var account = CloudStorageAccount.Parse(connectionString);
            var client = account.CreateCloudBlobClient();
            var container = client.GetContainerReference(containerName);

            return container;
        }
        public static Uri GetBlobUri(string containerName, string blobName)
        {
            var container = GetBlobContainer(containerName);
            var blob = container.GetBlockBlobReference(blobName);

            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
            sasConstraints.SharedAccessStartTime = DateTimeOffset.UtcNow.AddMinutes(-5);
            sasConstraints.SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddHours(24);
            sasConstraints.Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write;

            string sasBlobToken = blob.GetSharedAccessSignature(sasConstraints);

            return new Uri(blob.Uri + sasBlobToken);
        }

        public static async Task<string> DownloadBlobText(string containerName, string blobName)
        {
            var container = GetBlobContainer(containerName);
            var blob = container.GetBlockBlobReference(blobName);

            try
            {
                string res = await blob.DownloadTextAsync();
                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }
        public static async Task<Stream> DownloadBlobStream(string containerName, string blobName)
        {
            var container = GetBlobContainer(containerName);
            var blob = container.GetBlockBlobReference(blobName);

            try
            {
                Stream res = null;
                await blob.DownloadToStreamAsync(res);
                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }

        public static async Task UploadBlobText(string containerName, string blobName, string content)
        {
            var container = GetBlobContainer(containerName);
            var blob = container.GetBlockBlobReference(blobName);

            try
            {
                await blob.UploadTextAsync(content);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static async Task UploadBlobStream(string containerName, string blobName, Stream content)
        {
            var container = GetBlobContainer(containerName);
            var blob = container.GetBlockBlobReference(blobName);

            try
            {
                await blob.UploadFromStreamAsync(content);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static async Task DeleteBlob(string containerName, string blobName)
        {
            var container = GetBlobContainer(containerName);
            var blob = container.GetBlockBlobReference(blobName);

            try
            {
                await blob.DeleteIfExistsAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"[DEBUG:CLOUD:DELETE '{e.Message}']");
            }
        }
        
        public static async Task<List<CloudBlockBlob>> GetBlobList(string containerName)
        {
            var container = GetBlobContainer(containerName);

            BlobContinuationToken token = null;

            List<CloudBlockBlob> blobList = new List<CloudBlockBlob>();
            try
            {
                do
                {
                    var responce = await container.ListBlobsSegmentedAsync(token);
                    token = responce.ContinuationToken;
                    foreach (var blob in responce.Results.OfType<CloudBlockBlob>())
                    {
                        blobList.Add(blob);
                    }
                } while (token != null);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[DEBUG:BLOB:ERROR '{e.Message}']");
            }
            return blobList;
        }
    }
}