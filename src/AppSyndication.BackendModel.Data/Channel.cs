using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace AppSyndication.BackendModel.Data
{
    public class Channel : TableEntity
    {
        public Channel()
        {
        }

        public Channel(string alias)
        {
            this.PartitionKey = alias;
        }

        public string Alias => this.PartitionKey;

        public string Id { get; set; }

        public string Description { get; set; }

        public DateTime? DownloadCountRecalculated { get; set; }
    }
}
