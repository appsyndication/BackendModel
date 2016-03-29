using System;

namespace AppSyndication.BackendModel.IndexedData
{
    public class Tag
    {
        public Tag() { }

        protected Tag(Tag copy)
        {
            this.Id = copy.Id;
            this.Channel = copy.Channel;
            this.Alias = copy.Alias;
            this.Name = copy.Name;
            this.Description = copy.Description;
            this.Keywords = copy.Keywords;
            this.ImageUri = copy.ImageUri;
            this.BlobJsonUri = copy.BlobJsonUri;
            this.BlobXmlUri = copy.BlobXmlUri;
            this.Version = copy.Version;
            this.DownloadCount = copy.DownloadCount;
            this.Updated = copy.Updated;
        }

        public string Id { get; set; }

        public string Channel { get; set; }

        public string Alias { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string[] Keywords { get; set; }

        public string ImageUri { get; set; }

        public string BlobJsonUri { get; set; }

        public string BlobXmlUri { get; set; }

        public string Version { get; set; }

        public int DownloadCount { get; set; }

        public DateTime Updated { get; set; }
    }
}