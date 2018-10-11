using GroupDocs.Comparison.MVC.Products.Comparison.Config;

namespace GroupDocs.Comparison.MVC.Products.Common.Config
{
    /// <summary>
    /// Global configuration
    /// </summary>
    public class GlobalConfiguration
    {
        public ServerConfiguration Server;
        public ApplicationConfiguration Application;
        public CommonConfiguration Common;       
        public ComparisonConfiguration Comparison;

        /// <summary>
        /// Get all configurations
        /// </summary>
        public GlobalConfiguration()
        {
            Server = new ServerConfiguration();
            Application = new ApplicationConfiguration();          
            Common = new CommonConfiguration();          
            Comparison = new ComparisonConfiguration();
        }
    }
}