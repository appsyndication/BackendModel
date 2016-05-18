using System.Threading.Tasks;
using AppSyndication.BackendModel.Data.Azure;
using Microsoft.WindowsAzure.Storage.Table;

namespace AppSyndication.BackendModel.Data
{
    public interface ITableBase
    {
        AzureBatch Batch();

        Task Create(ITableEntity entity);

        Task CreateOrMergeAsync(ITableEntity entity);

        Task Update(ITableEntity entity);

        Task Upsert(ITableEntity entity);

        Task Delete(ITableEntity entity);

        Task<TableResult> ExecuteAsync(TableOperation operation);
    }
}