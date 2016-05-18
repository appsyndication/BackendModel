using Microsoft.WindowsAzure.Storage;

namespace AppSyndication.BackendModel.Data
{
    public interface ITagStorageConnection
    {
        CloudStorageAccount ConnectToIndexStorage();

        CloudStorageAccount ConnectToTagStorage();
    }
}