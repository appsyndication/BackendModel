using System.Collections.Generic;
using System.Threading.Tasks;
using AppSyndication.BackendModel.Data.Azure;
using Microsoft.WindowsAzure.Storage.Table;

namespace AppSyndication.BackendModel.Data
{
    public abstract class TableBase : ITableBase
    {
        private bool _exists;

        protected TableBase(string tableName, ITagStorageConnection connection)
        {
            var tables = connection.ConnectToTagStorage().CreateCloudTableClient();

            this.Table = tables.GetTableReference(tableName);
        }

        protected CloudTable Table { get; }

        public AzureBatch Batch()
        {
            return new AzureBatch(this);
        }

        public async Task Create(ITableEntity entity)
        {
            var op = TableOperation.Insert(entity);

            await this.ExecuteAsync(op);
        }

        public async Task CreateOrMergeAsync(ITableEntity entity)
        {
            var op = TableOperation.InsertOrMerge(entity);

            await this.ExecuteAsync(op);
        }

        public async Task Update(ITableEntity entity)
        {
            var op = TableOperation.Merge(entity);

            await this.ExecuteAsync(op);
        }

        public async Task Upsert(ITableEntity entity)
        {
            var op = TableOperation.InsertOrReplace(entity);

            await this.ExecuteAsync(op);
        }

        public async Task Delete(ITableEntity entity)
        {
            var op = TableOperation.Delete(entity);

            await this.ExecuteAsync(op);
        }

        public async Task<TableResult> ExecuteAsync(TableOperation operation)
        {
            await EnsureTableExistsAsync();

            return await this.Table.ExecuteAsync(operation);
        }

        public async Task<IList<TableResult>> ExecuteBatchAsync(TableBatchOperation batch)
        {
            await EnsureTableExistsAsync();

            return await this.Table.ExecuteBatchAsync(batch);
        }

        private async Task EnsureTableExistsAsync()
        {
            if (!_exists)
            {
                await this.Table.CreateIfNotExistsAsync();

                _exists = true;
            }
        }
    }
}
