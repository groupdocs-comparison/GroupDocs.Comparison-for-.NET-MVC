using GroupDocs.Comparison.MVC.Products.Common.Entity.Web;
using System.Collections.Generic;
using System.IO;

namespace GroupDocs.Comparison.MVC.Products.Comparison.Model.Request
{
    public class CompareRequest : PostedDataEntity
    {
        /// <summary>
        /// path or url for first file
        /// </summary>
        public string firstPath {get; set;}

        /// <summary>
        /// path or url for second file
        /// </summary>
        public string secondPath {get; set;}

        /// <summary>
        /// Password for first file
        /// </summary>
        public string firstPassword {get; set;}

        /// <summary>
        /// Password for second file
        /// </summary>
        public string secondPassword {get; set;}      
        
        /// <summary>
        /// Contains files stream if more than 2
        /// </summary>
        public List<Stream> files { get; set; }

        /// <summary>
        /// Contains list of the documents URLs
        /// </summary>
        public List<CompareFileDataRequest> urls { get; set; }

        /// <summary>
        /// Contains list of the documents paths
        /// </summary>
        public List<CompareFileDataRequest> paths { get; set; }

        /// <summary>
        /// Contains list of the documents passwords
        /// </summary>
        public List<string> passwords { get; set; }
    }
}