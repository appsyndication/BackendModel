using System;
using System.Collections.Generic;

namespace AppSyndication.BackendModel.Data
{
    public interface IDownloadTable : ITableBase
    {
        IEnumerable<DownloadEntity> GetDownloadsSince(DateTime? start);
    }
}