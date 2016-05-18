using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;

namespace AppSyndication.BackendModel.Data
{
    public interface ITagBlobContainer
    {
        string Name { get; }

        CloudStorageAccount StorageAccount();

        Task UploadChannelIndexJsonTag(string channel, string indexContent);

        Task UploadChannelIndexXmlTag(string channel, string indexContent);

        Task UploadJsonTag(TagEntity tagEntity, string tagContent);

        Task UploadXmlTag(TagEntity tagEntity, string tagContent);
    }
}