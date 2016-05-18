using System.IO;
using System.Threading.Tasks;

namespace AppSyndication.BackendModel.Data
{
    public interface ITagTransactionBlobContainer
    {
        Task<string> DownloadTextAsync(TagTransactionEntity tagTx);

        Task UploadFromStreamAsync(TagTransactionEntity tagTxEntity, Stream tagContent);
    }
}