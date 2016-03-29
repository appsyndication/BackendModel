
namespace AppSyndication.BackendModel.Data
{
    public class IndexChannelMessage
    {
        public IndexChannelMessage(string channel)
        {
            this.Channel = channel;
        }

        public string Channel { get; set; }
    }
}