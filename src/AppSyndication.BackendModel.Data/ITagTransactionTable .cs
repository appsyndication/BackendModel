using System.Threading.Tasks;

namespace AppSyndication.BackendModel.Data
{
    public interface ITagTransactionTable : ITableBase
    {
        TransactionSystemInfoEntity GetSystemInfo();

        Task<TagTransactionEntity> GetTagTransactionAsync(string channel, string transactionId);

        Task AddTagTransactionErrorMessageAsync(string channel, string transactionId, string message);

        Task AddTagTransactionErrorMessageAsync(TagTransactionEntity entity, string message);

        //Task<StartTagTransaction> StartTransactionAsync(string channel, string name);
    }
}