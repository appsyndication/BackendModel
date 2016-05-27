using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppSyndication.BackendModel.Data
{
    public interface IDownloadTable : ITableBase
    {
        IEnumerable<DownloadEntity> GetDownloadsSince(DateTime? start);

        Task IncrementDownloadRedirectCountAsync(string key, string ip);
    }
}