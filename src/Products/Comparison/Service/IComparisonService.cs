using GroupDocs.Comparison.MVC.Products.Common.Entity.Web;
using GroupDocs.Comparison.MVC.Products.Comparison.Model.Request;
using GroupDocs.Comparison.MVC.Products.Comparison.Model.Response;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace GroupDocs.Comparison.MVC.Products.Comparison.Service
{
    public interface IComparisonService
    {
        List<FileDescriptionEntity> LoadFiles(PostedDataEntity fileTreeRequest);

        /// <summary>
        /// Convert FormData object to CompareRequest object
        /// </summary>
        /// <param name="request">HttpRequest</param>
        /// <returns></returns>
        CompareRequest GetFormData(HttpRequest request);

        /// <summary>
        /// Compare two documents, save results in files
        /// </summary>
        /// <param name="compareRequest">PostedDataEntity</param>
        /// <returns>CompareResultResponse</returns>
        CompareResultResponse Compare(CompareRequest compareRequest);

        /// <summary>
        /// Compare two documents, save results in files,
        /// </summary>
        /// <param name="firstContent">Stream</param>
        /// <param name="firstPassword">string</param>
        /// <param name="secondContent">Stream</param>
        /// <param name="secondPassword">string</param>
        /// <param name="fileExt">string</param>
        /// <returns>CompareResultResponse</returns>
        CompareResultResponse CompareFiles(Stream firstContent, string firstPassword, Stream secondContent, string secondPassword, string fileExt);

        /// <summary>
        ///  Load the page of results
        /// </summary>
        /// <param name="loadResultPageRequest">PostedDataEntity</param>
        /// <returns>LoadedPageEntity</returns>
        PageDescriptionEntity LoadResultPage(PostedDataEntity loadResultPageRequest);

        /// <summary>
        ///  Produce file names for results
        /// </summary>
        /// <param name="documentGuid">string</param>
        /// <param name="index">int</param>
        /// <param name="ext">string</param>
        /// <returns>string</returns>
        string CalculateResultFileName(string documentGuid, string index, string ext);

        /// <summary>
        /// Check format files for comparing
        /// </summary>
        /// <param name="firstFileName">string</param>
        /// <param name="secondFileName">string</param>
        /// <returns>bool</returns>
        bool CheckFiles(string firstFileName, string secondFileName);

        /// <summary>
        /// Compare several files
        /// </summary>
        /// <param name="files">List[Stream]</param>
        /// <param name="passwords">List[string]</param>
        /// <param name="ext">string</param>
        /// <returns>CompareResultResponse</returns>
        CompareResultResponse MultiCompareFiles(List<Stream> files, List<string> passwords, string ext);

        /// <summary>
        /// Check format files for comparing
        /// </summary>
        /// <param name="fileNames">List[string]</param>
        /// <returns>bool</returns>
        bool CheckMultiFiles(List<string> fileNames);
    }
}