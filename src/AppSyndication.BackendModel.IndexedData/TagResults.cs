using System.Collections.Generic;

namespace AppSyndication.BackendModel.IndexedData
{
    public class TagResults
    {
        public string Query { get; set; }

        public int Page { get; set; }

        public int PageCount { get; set; }

        public int PerPage { get; set; }

        public int ResultsCount { get; set; }

        public int TotalTagsCount { get; set; }

        public IEnumerable<Tag> Tags { get; set; }
    }
}
