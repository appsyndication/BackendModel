using System.Collections.Generic;

namespace AppSyndication.BackendModel.IndexedData
{
    public interface ITagIndex
    {
        void Clear();

        Tag GetTag(string id);

        Tag GetTagByAlias(string tagAlias);

        Tag GetTagByAliasOrId(string aliasOrId);

        IEnumerable<TagHistory> GetTagHistory(string id);

        string GetTagUri(string id, bool xml = false);

        TagResults GetTags(int page = 0, int perPage = 0);

        TagResults SearchTags(string query, int page = 0, int perPage = 0);

        IEnumerable<DownloadRedirect> GetTagDownloadRedirects(string id, string version);
    }
}
