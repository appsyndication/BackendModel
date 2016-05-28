using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppSyndication.BackendModel.Data
{
    public interface ITagTable : ITableBase
    {
        Task<IEnumerable<TagEntity>> GetAllTagsAsync();

        Task<TagEntity> GetTagAsync(string partitionKey, string rowKey);

        Task<TagEntity> GetPrimaryTagAsync(TagEntity tag);
    }
}