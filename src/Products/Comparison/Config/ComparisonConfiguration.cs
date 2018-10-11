using System;
using System.Collections.Specialized;
using System.Configuration;

namespace GroupDocs.Comparison.MVC.Products.Comparison.Config
{
    /// <summary>
    /// CommonConfiguration
    /// </summary>
    public class ComparisonConfiguration : ConfigurationSection
    {
        public string FilesDirectory { get; set; }
        public string ResultDirectory { get; set; }
        public int PreloadResultPageCount { get; set; }
        public bool isMultiComparing { get; set; }       
        private NameValueCollection comparisonConfiguration = (NameValueCollection)System.Configuration.ConfigurationManager.GetSection("comparisonConfiguration");

        /// <summary>
        /// Constructor
        /// </summary>
        public ComparisonConfiguration()
        {
            // get Comparison configuration section from the web.config           
            FilesDirectory = comparisonConfiguration["filesDirectory"];
            ResultDirectory = comparisonConfiguration["resultDirectory"];
            PreloadResultPageCount = Convert.ToInt32(comparisonConfiguration["preloadResultPageCount"]);
            isMultiComparing = Convert.ToBoolean(comparisonConfiguration["isMultiComparing"]);         
        }
    }
}