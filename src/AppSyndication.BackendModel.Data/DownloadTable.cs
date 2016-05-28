using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppSyndication.BackendModel.Data.Azure;
using Microsoft.WindowsAzure.Storage.Table;

namespace AppSyndication.BackendModel.Data
{
    internal class DownloadTable : TableBase, IDownloadTable
    {
        public DownloadTable(ITagStorageConnection connection)
            : base(StorageName.DownloadTable, connection)
        {
        }

        public async Task<IEnumerable<DownloadEntity>> GetDownloadsSinceAsync(DateTime? start)
        {
            var startOffset = new DateTimeOffset(start ?? AzureDateTime.Min);

            var filter = TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.GreaterThan, startOffset);

            var query = new TableQuery<DownloadEntity>().Where(filter);

            return await this.ExecuteQueryAsync(query);
        }

        public async Task IncrementDownloadRedirectCountAsync(string key, string ip)
        {
            var now = DateTime.UtcNow;

            var download = new DownloadEntity(now, key, ip);

            await this.CreateOrMergeAsync(download);
        }
    }
}
