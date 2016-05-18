using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace AppSyndication.BackendModel.Data
{
    internal class TagQueue : ITagQueue
    {
        private bool _indexQueueAlreadyExists;
        private bool _tagTransactionQueueAlreadyExists;

        public TagQueue(ITagStorageConnection connection)
        {
            this.Connection = connection;
        }

        public ITagStorageConnection Connection { get; }

        public async Task EnqueueMessageAsync(StoreTagMessage content)
        {
            await this.QueueMessageAsync(StorageName.TagTransactionQueue, content, _tagTransactionQueueAlreadyExists);

            _tagTransactionQueueAlreadyExists = true;
        }

        public async Task EnqueueMessageAsync(IndexChannelMessage content)
        {
            await this.QueueMessageAsync(StorageName.IndexQueue, content, _indexQueueAlreadyExists);

            _indexQueueAlreadyExists = true;
        }

        private async Task QueueMessageAsync(string queueName, object content, bool alreadyExists)
        {
            var json = JsonConvert.SerializeObject(content);

            var message = new CloudQueueMessage(json);

            var queues = this.Connection.ConnectToTagStorage().CreateCloudQueueClient();

            var queue = queues.GetQueueReference(queueName);

            if (!alreadyExists)
            {
                await queue.CreateIfNotExistsAsync();
            }

            await queue.AddMessageAsync(message);
        }
    }
}
