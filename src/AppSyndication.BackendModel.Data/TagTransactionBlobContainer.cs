using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AppSyndication.BackendModel.Data
{
    internal class TagTransactionBlobContainer : ITagTransactionBlobContainer
    {
        private ITagStorageConnection _connection;

        private CloudBlobContainer _container;

        public TagTransactionBlobContainer(ITagStorageConnection connection)
        {
            _connection = connection;
        }

        public async Task<string> DownloadTextAsync(TagTransactionEntity tagTx)
        {
            var container = await this.ConnectAsync();

            var blob = container.GetBlockBlobReference(tagTx.StagedBlobUri);

            return await blob.DownloadTextAsync();
        }

        public async Task UploadFromStreamAsync(TagTransactionEntity tagTx, Stream stream)
        {
            var container = await this.ConnectAsync();

            var blob = container.GetBlockBlobReference(tagTx.StagedBlobUri);

            blob.Properties.ContentType = "application/swid-tag";
            blob.Metadata.Add("channel", tagTx.Channel);
            blob.Metadata.Add("alias", tagTx.Alias);
            blob.Metadata.Add("username", tagTx.Username);

            await blob.UploadFromStreamAsync(stream);
        }

        private async Task<CloudBlobContainer> ConnectAsync()
        {
            if (_container == null)
            {
                _container = _connection.ConnectToIndexStorage()
                    .CreateCloudBlobClient()
                    .GetContainerReference(StorageName.TagTransactionBlobContainer);

                await _container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Off, new BlobRequestOptions() { DisableContentMD5Validation = true, }, new OperationContext());
            }

            return _container;
        }
    }
}
