using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppSyndication.BackendModel.Data
{
    public interface IDownloadTable : ITableBase
    {
        Task<IEnumerable<DownloadEntity>> GetDownloadsSinceAsync (DateTime? start);

        Task IncrementDownloadRedirectCountAsync(string key, string ip);
    }
}