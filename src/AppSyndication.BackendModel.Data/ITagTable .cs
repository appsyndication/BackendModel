using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppSyndication.BackendModel.Data
{
    public interface ITagTable : ITableBase
    {
        IEnumerable<TagEntity> GetAllTags();

        Task<TagEntity> GetTagAsync(string partitionKey, string rowKey);

        Task<TagEntity> GetPrimaryTagAsync(TagEntity tag);
    }
}