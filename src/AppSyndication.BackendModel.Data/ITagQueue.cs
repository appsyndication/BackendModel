using System.Threading.Tasks;

namespace AppSyndication.BackendModel.Data
{
    public interface ITagQueue
    {
        Task EnqueueMessageAsync(StoreTagMessage content);

        Task EnqueueMessageAsync(IndexChannelMessage content);
    }
}