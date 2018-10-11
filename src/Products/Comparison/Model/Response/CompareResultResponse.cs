using GroupDocs.Comparison.Common.Changes;
using System.Collections.Generic;

namespace GroupDocs.Comparison.MVC.Products.Comparison.Model.Response
{
    public class CompareResultResponse
    {
        /// <summary>
        /// List of changies
        /// </summary>
        public ChangeInfo[] changes;

        /// <summary>
        /// List of images of pages with marked changes
        /// </summary>
        public List<string> pages;

        /// <summary>
        /// Unique key of results
        /// </summary>
        public string guid;

        /// <summary>
        /// Extension of compared files, for saving total results
        /// </summary>
        public string extension;
    }
}