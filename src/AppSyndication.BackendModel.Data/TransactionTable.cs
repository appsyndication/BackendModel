using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace AppSyndication.BackendModel.Data
{
    internal class TransactionTable : TableBase, ITagTransactionTable
    {
        public TransactionTable(ITagStorageConnection connection)
            : base(StorageName.TransactionTable, connection)
        {
        }

        public TransactionSystemInfoEntity GetSystemInfo()
        {
            var op = TableOperation.Retrieve<TransactionSystemInfoEntity>(TransactionSystemInfoEntity.PartitionKeyValue, TransactionSystemInfoEntity.RowKeyValue);

            var result = this.Table.Execute(op);

            return (TransactionSystemInfoEntity)result.Result ?? new TransactionSystemInfoEntity();
        }

        public RedirectEntity GetRedirect(string redirectKey)
        {
            var op = TableOperation.Retrieve<RedirectEntity>(redirectKey, String.Empty);

            var result = this.Table.Execute(op);

            return (RedirectEntity)result.Result;
        }

        public async Task<TagTransactionEntity> GetTagTransactionAsync(string channel, string transactionId)
        {
            var partitionKey = TagTransactionEntity.CalculatePartitionKey(channel, transactionId);

            var rowKey = TagTransactionEntity.CalculateRowKey();

            var op = TableOperation.Retrieve<TagTransactionEntity>(partitionKey, rowKey);

            var result = await this.Table.ExecuteAsync(op);

            return (TagTransactionEntity)result.Result;
        }

        public async Task AddTagTransactionErrorMessageAsync(string channel, string transactionId, string message)
        {
            var txTag = await this.GetTagTransactionAsync(channel, transactionId);

            if (txTag != null)
            {
                await AddTagTransactionErrorMessageAsync(txTag, message);
            }
        }

        public async Task AddTagTransactionErrorMessageAsync(TagTransactionEntity entity, string message)
        {
            var change = this.Batch();

            if (entity.TryUpdateOperation(TagTransactionOperation.Error))
            {
                var errorEntity = entity.AsError();

                change.Update(errorEntity);
            }

            var messageEntity = new TagTransactionMessageEntity(entity, message);

            change.Create(messageEntity);

            await change.WhenAll();
        }
    }
}
