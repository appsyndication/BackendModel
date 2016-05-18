using System.Diagnostics;
using Microsoft.WindowsAzure.Storage;

namespace AppSyndication.BackendModel.Data
{
    [DebuggerDisplay("ITagStorageConnection \"{StorageConnectionString}\", nq")]
    public class Connection : ITagStorageConnection
    {
        public Connection(string storageConnectionString)
        {
            this.StorageConnectionString = storageConnectionString;

            this.TagStorage = CloudStorageAccount.Parse(this.StorageConnectionString);
        }

        public string StorageConnectionString { get; }

        private CloudStorageAccount TagStorage { get; }

        public CloudStorageAccount ConnectToIndexStorage()
        {
            return this.ConnectToTagStorage();
        }

        public CloudStorageAccount ConnectToTagStorage()
        {
            return this.TagStorage;
        }

        //public async Task<CloudBlobContainer> TagContainerAsync(bool ensureExists = true)
        //{
        //    var container = this.AccessBlobs().GetContainerReference(StorageName.TagBlobContainer);

        //    if (ensureExists && !_tagContainerAlreadyExists)
        //    {
        //        await container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, new BlobRequestOptions() { DisableContentMD5Validation = true, }, new OperationContext());
        //        _tagContainerAlreadyExists = true;
        //    }

        //    return container;
        //}

        //public async Task<CloudBlobContainer> TagTransactionContainerAsync(bool ensureExists = true)
        //{
        //    var container = this.AccessBlobs().GetContainerReference(StorageName.TagTransactionBlobContainer);

        //    if (ensureExists && !_tagTransactionContainerAlreadyExists)
        //    {
        //        await container.CreateIfNotExistsAsync();
        //        _tagTransactionContainerAlreadyExists = true;
        //    }

        //    return container;
        //}

        //public async Task<CloudBlockBlob> TagTransactionUploadBlobAsync(string channel, string transactionId)
        //{
        //    var blobName = $"{channel}/{transactionId}";

        //    var container = await this.TagTransactionContainerAsync();

        //    return container.GetBlockBlobReference(blobName);
        //}

        //public async Task QueueTagTransactionMessageAsync(StoreTagMessage content)
        //{
        //    await this.QueueMessageAsync(StorageName.TagTransactionQueue, content, _tagTransactionQueueAlreadyExists);

        //    _tagTransactionQueueAlreadyExists = true;
        //}

        //public async Task QueueIndexMessageAsync(IndexChannelMessage content)
        //{
        //    await this.QueueMessageAsync(StorageName.IndexQueue, content, _indexQueueAlreadyExists);

        //    _indexQueueAlreadyExists = true;
        //}

        //public virtual RedirectTable RedirectTable()
        //{
        //    return new RedirectTable(this);
        //}

        //public virtual DownloadTable DownloadTable()
        //{
        //    return new DownloadTable(this);
        //}

        //public virtual TagTable TagTable()
        //{
        //    return new TagTable(this);
        //}

        //public virtual TransactionTable TransactionTable()
        //{
        //    return new TransactionTable(this);
        //}

        //internal CloudTableClient AccessTables()
        //{
        //    return this.ConnectToTagStorage().CreateCloudTableClient();
        //}

        //private CloudBlobClient AccessBlobs()
        //{
        //    return this.ConnectToTagStorage().CreateCloudBlobClient();
        //}

        //private async Task QueueMessageAsync(string queueName, object content, bool alreadyExists)
        //{
        //    var json = JsonConvert.SerializeObject(content);

        //    var message = new CloudQueueMessage(json);

        //    var queues = this.ConnectToTagStorage().CreateCloudQueueClient();

        //    var queue = queues.GetQueueReference(queueName);

        //    if (!alreadyExists)
        //    {
        //        await queue.CreateIfNotExistsAsync();
        //    }

        //    await queue.AddMessageAsync(message);
        //}
    }
}
