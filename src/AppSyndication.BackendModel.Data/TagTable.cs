using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace AppSyndication.BackendModel.Data
{
    internal class TagTable : TableBase, ITagTable
    {
        public TagTable(ITagStorageConnection connection)
            : base(StorageName.TagTable, connection)
        {
        }

        public async Task<TagEntity> GetTagAsync(string partitionKey, string rowKey)
        {
            var op = TableOperation.Retrieve<TagEntity>(partitionKey, rowKey);

            var result = await this.Table.ExecuteAsync(op);

            return (TagEntity)result.Result;
        }

        public async Task<TagEntity> GetPrimaryTagAsync(TagEntity tag)
        {
            var partitionKey = TagEntity.CalculatePartitionKey(tag.Channel);

            var rowKey = TagEntity.CalculateRowKey(true, tag.Alias, tag.Media, tag.Version, tag.Revision);

            var op = TableOperation.Retrieve<TagEntity>(partitionKey, rowKey);

            var result = await this.Table.ExecuteAsync(op);

            return (TagEntity)result.Result;
        }

        public async Task<IEnumerable<TagEntity>> GetAllPrimaryTagsAsync()
        {
            var query = new TableQuery<TagEntity>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, String.Empty));

            var tags = await this.ExecuteQueryAsync(query);

            return tags;
        }

        public async Task<IEnumerable<TagEntity>> GetAllTagsAsync()
        {
            var query = new TableQuery<TagEntity>();

            var tags = await this.ExecuteQueryAsync(query);

            return tags;
        }
    }
}
