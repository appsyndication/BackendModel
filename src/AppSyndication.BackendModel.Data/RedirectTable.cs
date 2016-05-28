using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace AppSyndication.BackendModel.Data
{
    internal class RedirectTable : TableBase, IRedirectTable
    {
        public RedirectTable(ITagStorageConnection connection)
            : base(StorageName.RedirectTable, connection)
        {
        }

        public async Task<IEnumerable<RedirectEntity>> GetAllRedirectsAsync()
        {
            var query = new TableQuery<RedirectEntity>();

            var results = await this.ExecuteQueryAsync(query);

            return results;
        }

        public async Task<RedirectEntity> GetRedirectAsync(string redirectKey)
        {
            var op = TableOperation.Retrieve<RedirectEntity>(redirectKey, String.Empty);

            var result = await this.Table.ExecuteAsync(op);

            return (RedirectEntity)result.Result;
        }
    }
}
