using System;

namespace AppSyndication.BackendModel.IndexedData
{
    public class TagHistory
    {
        public string Title { get; set; }

        public string Version { get; set; }

        public int DownloadCount { get; set; }

        public DateTime Updated { get; set; }
    }
}