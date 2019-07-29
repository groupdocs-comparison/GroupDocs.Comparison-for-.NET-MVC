using System;
using System.IO;
using System.Linq;
using GroupDocs.Comparison.MVC.Products.Common.Config;
using GroupDocs.Comparison.MVC.Products.Common.Util.Parser;

namespace GroupDocs.Comparison.MVC.Products.Comparison.Config
{
    /// <summary>
    /// CommonConfiguration
    /// </summary>
    public class ComparisonConfiguration : CommonConfiguration
    {
        private string FilesDirectory = "DocumentSamples/Comparison";
        private string ResultDirectory = "DocumentSamples/Comparison/Compared";
        private int PreloadResultPageCount = 0;
        private bool isMultiComparing = true;

        /// <summary>
        /// Constructor
        /// </summary>
        public ComparisonConfiguration()
        {
            YamlParser parser = new YamlParser();
            dynamic configuration = parser.GetConfiguration("comparison");
            ConfigurationValuesGetter valuesGetter = new ConfigurationValuesGetter(configuration);
            // get Comparison configuration section from the web.config            
            FilesDirectory = valuesGetter.GetStringPropertyValue("filesDirectory", FilesDirectory);
            if (!IsFullPath(FilesDirectory))
            {
                FilesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FilesDirectory);
                if (!Directory.Exists(FilesDirectory))
                {
                    Directory.CreateDirectory(FilesDirectory);
                }
            }
            ResultDirectory = valuesGetter.GetStringPropertyValue("resultDirectory", ResultDirectory);
            if (!IsFullPath(ResultDirectory))
            {
                ResultDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ResultDirectory);
                if (!Directory.Exists(ResultDirectory))
                {
                    Directory.CreateDirectory(ResultDirectory);
                }
            }
            PreloadResultPageCount = valuesGetter.GetIntegerPropertyValue("preloadResultPageCount", PreloadResultPageCount);
            isMultiComparing = valuesGetter.GetBooleanPropertyValue("multiComparing", isMultiComparing);
        }

        private static bool IsFullPath(string path)
        {
            return !String.IsNullOrWhiteSpace(path)
                && path.IndexOfAny(System.IO.Path.GetInvalidPathChars().ToArray()) == -1
                && Path.IsPathRooted(path)
                && !Path.GetPathRoot(path).Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal);
        }

        public void SetFilesDirectory(string filesDirectory)
        {
            this.FilesDirectory = filesDirectory;
        }

        public string GetFilesDirectory()
        {
            return FilesDirectory;
        }

        public void SetResultDirectory(string resultDirectory)
        {
            this.ResultDirectory = resultDirectory;
        }

        public string GetResultDirectory()
        {
            return ResultDirectory;
        }

        public void SetPreloadResultPageCounty(int preloadResultPageCount)
        {
            this.PreloadResultPageCount = preloadResultPageCount;
        }

        public int GetPreloadResultPageCount()
        {
            return PreloadResultPageCount;
        }

        public void SetIsMultiComparing(bool isMultiComparing)
        {
            this.isMultiComparing = isMultiComparing;
        }

        public bool GetIsMultiComparing()
        {
            return isMultiComparing;
        }
    }
}