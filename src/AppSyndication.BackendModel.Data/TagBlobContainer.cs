using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AppSyndication.BackendModel.Data
{
    internal class TagBlobContainer : ITagBlobContainer
    {
        private ITagStorageConnection _connection;

        private CloudBlobContainer _container;

        public TagBlobContainer(ITagStorageConnection connection)
        {
            _connection = connection;
        }

        public string Name => StorageName.TagBlobContainer;

        public CloudStorageAccount StorageAccount()
        {
            return _connection.ConnectToTagStorage();
        }

        public async Task UploadChannelIndexJsonTag(string channel, string indexContent)
        {
            await this.UploadChannelIndexTag(channel, indexContent, "index.json.swidtag", "application/swid-tag+json");
        }

        public async Task UploadChannelIndexXmlTag(string channel, string indexContent)
        {
            await this.UploadChannelIndexTag(channel, indexContent, "index.xml.swidtag", "application/swid-tag+xml");
        }

        public async Task UploadJsonTag(TagEntity tagEntity, string tagContent)
        {
            await UploadTagToBlob(tagEntity, tagEntity.JsonBlobName, tagContent, "application/swid-tag+json");
        }

        public async Task UploadXmlTag(TagEntity tagEntity, string tagContent)
        {
            await UploadTagToBlob(tagEntity, tagEntity.XmlBlobName, tagContent, "application/swid-tag+xml");
        }

        private async Task<CloudBlobContainer> ConnectAsync()
        {
            if (_container == null)
            {
                _container = _connection.ConnectToIndexStorage()
                    .CreateCloudBlobClient()
                    .GetContainerReference(StorageName.TagBlobContainer);

                await _container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, new BlobRequestOptions() { DisableContentMD5Validation = true, }, new OperationContext());
            }

            return _container;
        }

        private async Task UploadChannelIndexTag(string channel, string tag, string tagName, string contentType)
        {
            var container = await this.ConnectAsync();

            var blob = container.GetBlockBlobReference(channel + "/" + tagName);

            var bytes = Encoding.UTF8.GetBytes(tag);

            await blob.UploadFromByteArrayAsync(bytes, 0, bytes.Length);

            blob.Properties.CacheControl = "public, max-age=300"; // cache for 5 minutes.

            blob.Properties.ContentType = contentType;

            await blob.SetPropertiesAsync();
        }

        private async Task UploadTagToBlob(TagEntity tag, string tagName, string content, string contentType)
        {
            var container = await this.ConnectAsync();

            var blob = container.GetBlockBlobReference(tagName);

            blob.Properties.ContentType = contentType;
            blob.Metadata.Add("id", tag.TagId);
            blob.Metadata.Add("uid", tag.Uid);
            blob.Metadata.Add("version", tag.Version);
            blob.Metadata.Add("revision", tag.Revision);

            // TODO: it would be nice if we could pre-gzip our tags in storage but that requires the client to accept
            //       gzip which not enough people seem to do.
            //blob.Properties.ContentEncoding = "gzip";
            //using (var stream = new MemoryStream(bytes.Length))
            //{
            //    using (var gzip = new GZipStream(stream, CompressionLevel.Optimal, true))
            //    {
            //        gzip.Write(bytes, 0, bytes.Length);
            //    }

            //    stream.Seek(0, SeekOrigin.Begin);
            //    blob.UploadFromStream(stream);
            //}

            var bytes = Encoding.UTF8.GetBytes(content);

            await blob.UploadFromByteArrayAsync(bytes, 0, bytes.Length);
        }
    }
}
