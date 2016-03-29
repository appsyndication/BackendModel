using System;
using System.Collections.Generic;
using System.Linq;
using AppSyndication.BackendModel.Data;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store.Azure;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace AppSyndication.BackendModel.IndexedData
{
    public class TagIndex : ITagIndex
    {
        private const string IndexSearchReaderCacheId = "search-reader";

        private static readonly string[] _fields = new[] { IndexFields.TagAlias, IndexFields.TagTitle, IndexFields.TagDescription, IndexFields.TagKeywords };

        private readonly IMemoryCache _cache;

        private readonly ILogger _logger;

        private readonly Connection _connection;

        private int _totalTags;

        public TagIndex(ILoggerFactory loggerFactory, IMemoryCache cache, Connection connection)
        {
            _logger = loggerFactory.CreateLogger(typeof(TagIndex).FullName);

            _cache = cache;

            _connection = connection;

            _totalTags = -1;
        }

        public void Clear()
        {
            _cache.Remove(IndexSearchReaderCacheId);
        }

        public Tag GetTag(string id)
        {
            var document = this.GetSingleDocumentForSearchTerm(IndexFields.TagUid, id);

            if (document != null)
            {
                return CreateTagFromDocument(document);
            }

            return null;
        }

        public Tag GetTagByAlias(string tagAlias)
        {
            var document = this.GetSingleDocumentForSearchTerm(IndexFields.TagAlias, tagAlias);

            if (document == null)
            {
                return null;
            }

            return CreateTagFromDocument(document);
        }

        public Tag GetTagByAliasOrId(string aliasOrId)
        {
            var document = this.GetSingleDocumentForSearchTerm(IndexFields.TagAlias, aliasOrId);

            if (document == null)
            {
                document = this.GetSingleDocumentForSearchTerm(IndexFields.TagUid, aliasOrId);

                if (document == null)
                {
                    return null;
                }
            }

            return CreateTagFromDocument(document);
        }

        public IEnumerable<TagHistory> GetTagHistory(string id)
        {
            var query = new TermQuery(new Term (IndexFields.HistoryTagUid, id));

            var histories = this.GetAllDocumentForQuery(query)
                .Select(CreateTagHistoryFromDocument);

            return histories;
        }

        public string GetTagUri(string id, bool xml = false)
        {
            var document = this.GetSingleDocumentForSearchTerm(IndexFields.TagUid, id);

            if (document != null)
            {
                return document.Get(xml ? IndexFields.TagBlobXmlUri : IndexFields.TagBlobJsonUri);
            }

            return null;
        }

        public TagResults GetTags(int page = 1, int perPage = 0)
        {
            var reader = this.GetIndexReader();

            if (page < 1)
            {
                page = 1;
            }

            if (perPage < 1)
            {
                perPage = _totalTags;
            }

            var start = (page - 1) * perPage;
            var end = Math.Min(page * perPage, _totalTags);

            var results = new TagResults();
            results.Page = page;
            results.PerPage = perPage;
            results.PageCount = (_totalTags + 1) / perPage;
            results.ResultsCount = _totalTags;
            results.TotalTagsCount = _totalTags;
            results.Tags = this.GetAllTagsFromReader(reader, start, end)
                .ToList();

            return results;
        }

        public TagResults SearchTags(string query, int page = 1, int perPage = 0)
        {
            var reader = this.GetIndexReader();

            if (page < 1)
            {
                page = 1;
            }

            if (perPage < 1)
            {
                perPage = _totalTags;
            }

            var start = (page - 1) * perPage;
            var end = Math.Min(page * perPage, _totalTags);

            using (var searcher = new IndexSearcher(reader))
            {
                var parsedQuery = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, _fields, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30))
                    .Parse(query);

                var searchResults = searcher.Search(parsedQuery, end);

                var results = new TagResults();
                results.Query = query;
                results.Page = page;
                results.PerPage = perPage;
                results.PageCount = (_totalTags + 1) / perPage;
                results.ResultsCount = searchResults.TotalHits;
                results.TotalTagsCount = _totalTags;
                results.Tags = GetAllTagsFromSearchResults(searcher, searchResults, start, end)
                    .ToList();

                return results;
            }
        }

        public IEnumerable<DownloadRedirect> GetTagDownloadRedirects(string id, string version)
        {
            var query = new BooleanQuery();

            query.Add(new TermQuery(new Term (IndexFields.RedirectTagUid, id)), Occur.MUST);

            query.Add(new TermQuery(new Term (IndexFields.RedirectTagVersion, version ?? String.Empty)), Occur.MUST);

            var results = this.GetAllDocumentForQuery(query);

            var redirects = results.Select(CreateDownloadRedirectFromDocument);

            return redirects.ToList();
        }

        private IndexReader GetIndexReader()
        {
            IndexReader reader;

            if (!_cache.TryGetValue(IndexSearchReaderCacheId, out reader))
            {
                _logger.LogInformation("Populating cache with index reader.");

                // Refresh it every 10 minutes no matter what or 2 minutes if unused. Let this be
                // the last item to be removed by cache if cache GC kicks in.
                var options = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(2),
                    Priority = CacheItemPriority.High,
                };

                var storage = _connection.ConnectToIndexStorage();
                var azureDirectory = new AzureDirectory(storage, "search-index");

                reader = IndexReader.Open(azureDirectory, true);

                _totalTags = GetCountOfDocumentsForSearchTerm(reader, IndexFields.DocumentType, DocumentTypes.Tag);

                _cache.Set(IndexSearchReaderCacheId, reader, options);
            }

            return reader;
        }

        private IEnumerable<Tag> GetAllTagsFromReader(IndexReader reader, int start, int end)
        {
            var query = new TermQuery(new Term (IndexFields.DocumentType, DocumentTypes.Tag));

            var tags = this.GetAllDocumentForQuery(query, start, end)
                .Select(CreateTagFromDocument);

            return tags;
        }

        private Document GetSingleDocumentForSearchTerm(string termField, string termValue)
        {
            var query = new TermQuery(new Term (termField, termValue));

            return this.GetSingleDocumentForQuery(query);
        }

        private Document GetSingleDocumentForTwoSearchTerms(string termField1, string termValue1, string termField2, string termValue2)
        {
            var query = new BooleanQuery();

            query.Add(new TermQuery(new Term (termField1, termValue1)), Occur.MUST);

            query.Add(new TermQuery(new Term (termField2, termValue2)), Occur.MUST);

            return this.GetSingleDocumentForQuery(query);
        }

        private IEnumerable<Document> GetAllDocumentForQuery(Query query, int start = 0, int end = 0, bool ordered = false)
        {
            var reader = this.GetIndexReader();

            using (var searcher = new IndexSearcher(reader))
            {
                var results = new AllSearchResults(ordered);

                searcher.Search(query, results);

                var docIds = results.DocIds;

                if (start > 0)
                {
                    docIds = docIds.Skip(start);
                }

                if (end > 0)
                {
                    docIds = docIds.Take(Math.Max(0, end - start));
                }

                foreach (var docId in docIds)
                {
                    yield return searcher.Doc(docId);
                }
            }
        }

        private Document GetSingleDocumentForQuery(Query query)
        {
            Document document = null;

            var reader = this.GetIndexReader();

            using (var searcher = new IndexSearcher(reader))
            {
                var results = searcher.Search(query, 1);

                if (results.TotalHits > 0)
                {
                    document = searcher.Doc(results.ScoreDocs[0].Doc);
                }
            }

            return document;
        }

        private static IEnumerable<Tag> GetAllTagsFromSearchResults(IndexSearcher searcher, TopDocs results, int start, int end)
        {
            var take = Math.Max(0, end - start);

            foreach (var scoreDoc in results.ScoreDocs.Skip(start).Take(take))
            {
                var document = searcher.Doc(scoreDoc.Doc);

                yield return CreateTagFromDocument(document);
            }
        }

        private static int GetCountOfDocumentsForSearchTerm(IndexReader reader, string termField, string termValue)
        {
            var query = new TermQuery(new Term (termField, termValue));

            using (var searcher = new IndexSearcher(reader))
            {
                var results = searcher.Search(query, 1);

                return results.TotalHits;
            }
        }

        private static Tag CreateTagFromDocument(Document doc)
        {
            var channel = doc.Get(IndexFields.TagChannel);

            var keywords = doc.Get(IndexFields.TagKeywords);

            var image = doc.Get(IndexFields.TagLogoUri);

            var downloads = 0;

            Int32.TryParse(doc.Get(IndexFields.TagDownloadCount), out downloads);

            var updated = DateTime.MinValue.ToUniversalTime();

            DateTime.TryParse(doc.Get(IndexFields.TagUpdated), out updated);

            return new Tag()
                {
                    Id  = doc.Get(IndexFields.TagUid),
                    Channel = String.IsNullOrEmpty(channel) ? null : channel,
                    Alias = doc.Get(IndexFields.TagAlias),
                    Description = doc.Get(IndexFields.TagDescription),
                    Keywords = String.IsNullOrEmpty(keywords) ? new string[0] : keywords.Split(','),
                    Name = doc.Get(IndexFields.TagTitle),
                    ImageUri = String.IsNullOrEmpty(image) ? "/images/appdefaulticon-50x50.png" : image,
                    BlobJsonUri = doc.Get(IndexFields.TagBlobJsonUri),
                    BlobXmlUri = doc.Get(IndexFields.TagBlobXmlUri),
                    Version = doc.Get(IndexFields.TagVersion),
                    DownloadCount = downloads,
                    Updated = updated,
                };
        }

        private static TagHistory CreateTagHistoryFromDocument(Document doc)
        {
            var downloads = 0;

            Int32.TryParse(doc.Get(IndexFields.HistoryTagDownloadCount), out downloads);

            var updated = DateTime.MinValue.ToUniversalTime();

            DateTime.TryParse(doc.Get(IndexFields.HistoryTagUpdated), out updated);

            return new TagHistory()
                {
                    Title = doc.Get(IndexFields.HistoryTagTitle),
                    Version = doc.Get(IndexFields.HistoryTagVersion),
                    DownloadCount = downloads,
                    Updated = updated,
                };
        }

        private static DownloadRedirect CreateDownloadRedirectFromDocument(Document doc)
        {
            return new DownloadRedirect()
                {
                    Key  = doc.Get(IndexFields.RedirectKey),
                    TagUid = doc.Get(IndexFields.RedirectTagUid),
                    TagVersion = doc.Get(IndexFields.RedirectTagVersion),
                    Media = doc.Get(IndexFields.RedirectMedia),
                    ContentType = doc.Get(IndexFields.RedirectContentType),
                    Uri = doc.Get(IndexFields.RedirectUri),
                };
        }

        private static class DocumentTypes
        {
            public const string History = "history";
            public const string Redirect = "redirect";
            public const string Tag = "tag";
        }

        private static class IndexFields
        {
            public const string DocumentType = "_type";

            public const string HistoryTagUid = "history_tagUid";
            public const string HistoryTagTitle = "history_tagTitle";
            public const string HistoryTagVersion = "history_tagVersion";
            public const string HistoryTagUpdated = "history_tagUpdated";
            public const string HistoryTagDownloadCount = "history_downloads";

            public const string TagUid = "tag_uid";
            public const string TagChannel = "tag_channel";
            public const string TagDownloadCount = "tag_downloads";
            public const string TagAlias = "tag_alias";
            public const string TagTitle = "tag_title";
            public const string TagDescription = "tag_description";
            public const string TagKeywords = "tag_keywords";
            public const string TagLogoUri = "tag_logoUri";
            public const string TagBlobJsonUri = "tag_blobJsonUri";
            public const string TagBlobXmlUri = "tag_blobXmlUri";
            public const string TagUpdated = "tag_updated";
            public const string TagVersion = "tag_version";

            public const string RedirectKey = "redirect_key";
            public const string RedirectTagUid = "redirect_tagUid";
            public const string RedirectTagVersion = "redirect_tagVersion";
            public const string RedirectMedia = "redirect_media";
            public const string RedirectContentType = "redirect_type";
            public const string RedirectUri = "redirect_uri";
        }

        private class AllSearchResults : Collector
        {
            private readonly bool _ordered;

            private int _docBase;

            private readonly List<int> _docIds = new List<int>();

            public AllSearchResults(bool ordered)
            {
                _ordered = ordered;
            }

            public override bool AcceptsDocsOutOfOrder => !_ordered;

            public IEnumerable<int> DocIds => _docIds;

            public override void Collect(int doc)
            {
                _docIds.Add(doc);
            }

            public override void SetNextReader(IndexReader reader, int docBase)
            {
                _docBase = docBase;
            }

            public override void SetScorer(Scorer scorer)
            {
            }
        }
    }
}
