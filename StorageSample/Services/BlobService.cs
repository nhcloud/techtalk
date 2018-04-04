using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using StorageSample.Models;
using Microsoft.Azure.KeyVault.Core;
using StorageSample.Helpers;

namespace StorageSample.Services
{
    public class BlobService : IFileService
    {
        private static readonly CloudBlobContainer BlobContainer;
        private static readonly IKey KeyVaultKey;

        static BlobService()
        {
            var storageConnection = Utility.GetSecret(AppSettings.StorageConnection);
            var blobClient = CloudStorageAccount.Parse(storageConnection).CreateCloudBlobClient();
            BlobContainer = blobClient.GetContainerReference(AppSettings.ContainerName);
            BlobContainer.CreateIfNotExists();
            KeyVaultKey = Utility.GetKey(AppSettings.KeyUri);
        }
        private static BlobRequestOptions GetBlobRequestOptions()
        {
            var policy = new BlobEncryptionPolicy(KeyVaultKey, null);
            return new BlobRequestOptions { EncryptionPolicy = policy };
        }
        public void WriteAllText(string contents, string path)
        {
            var blob = BlobContainer.GetBlockBlobReference(path);
            blob.UploadText(contents, options: GetBlobRequestOptions());
        }
        public bool Exists(string path)
        {
            var blob = BlobContainer.GetBlobReference(path);
            try
            {
                blob.FetchAttributes();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public long Length(string path)
        {
            var blob = BlobContainer.GetBlobReference(path);
            try
            {
                blob.FetchAttributes();
            }
            catch (Exception)
            {
                // ignored
            }
            var blobProperties = blob.Properties;
            return blobProperties.Length;
        }
        public string ReadAllText(string path)
        {
            var response = "";
            var blob = BlobContainer.GetBlockBlobReference(path);
            if (blob.Exists())
            {
                response = blob.DownloadText(options: GetBlobRequestOptions());
            }
            return response;
        }
    }
}