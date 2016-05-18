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

        public IEnumerable<RedirectEntity> GetAllRedirects()
        {
            var query = new TableQuery<RedirectEntity>();

            var results = this.Table.ExecuteQuery(query);

            return results;
        }

        public RedirectEntity GetRedirect(string redirectKey)
        {
            var op = TableOperation.Retrieve<RedirectEntity>(redirectKey, String.Empty);

            var result = this.Table.Execute(op);

            return (RedirectEntity)result.Result;
        }

        public async Task<RedirectEntity> GetRedirectAsync(string redirectKey)
        {
            var op = TableOperation.Retrieve<RedirectEntity>(redirectKey, String.Empty);

            var result = await this.Table.ExecuteAsync(op);

            return (RedirectEntity)result.Result;
        }
    }
}
