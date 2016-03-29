using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppSyndication.BackendModel.Data.Azure;
using Microsoft.WindowsAzure.Storage.Table;

namespace AppSyndication.BackendModel.Data
{
    public class DownloadTable : TableBase
    {
        public DownloadTable(Connection connection, bool ensureExists, ref bool alreadyExists)
            : base(StorageName.DownloadTable, connection, ensureExists, ref alreadyExists)
        {
        }

        public virtual IEnumerable<DownloadEntity> GetDownloadsSince(DateTime? start)
        {
            var startOffset = new DateTimeOffset(start ?? AzureDateTime.Min);

            var filter = TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.GreaterThan, startOffset);

            var query = new TableQuery<DownloadEntity>().Where(filter);

            return this.Table.ExecuteQuery(query);
        }

        public async Task IncrementDownloadRedirectCountAsync(string key, string ip)
        {
            var now = DateTime.UtcNow;

            var download = new DownloadEntity(now, key, ip);

            await this.CreateOrMergeAsync(download);
        }
    }
}
