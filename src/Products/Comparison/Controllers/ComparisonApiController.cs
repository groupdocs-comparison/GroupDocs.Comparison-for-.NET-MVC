using GroupDocs.Comparison.MVC.Products.Common.Entity.Web;
using GroupDocs.Comparison.MVC.Products.Common.Resources;
using GroupDocs.Comparison.MVC.Products.Comparison.Model.Request;
using GroupDocs.Comparison.MVC.Products.Comparison.Model.Response;
using GroupDocs.Comparison.MVC.Products.Comparison.Service;
using GroupDocs.Comparison.MVC.Products.Comparison.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace GroupDocs.Comparison.MVC.Products.Comparison.Controllers
{
    /// <summary>
    /// SignatureApiController
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ComparisonApiController : ApiController
    {
        private IComparisonService comparisonService;
        private static Common.Config.GlobalConfiguration globalConfiguration;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="globalConfiguration">GlobalConfiguration</param>
        public ComparisonApiController()
        {
            globalConfiguration = new Common.Config.GlobalConfiguration();
            comparisonService = new ComparisonServiceImpl(globalConfiguration);
        }



        /// <summary>
        /// Get all files and directories from storage
        /// </summary>
        /// <param name="postedData">Post data</param>
        /// <returns>List of files and directories</returns>
        [HttpPost]
        [Route("loadFileTree")]
        public HttpResponseMessage loadFileTree(PostedDataEntity fileTreeRequest)
        {
            return Request.CreateResponse(HttpStatusCode.OK, comparisonService.LoadFiles(fileTreeRequest));
        }

        /// <summary>
        /// Download results
        /// </summary>
        /// <param name=""></param>
        [HttpGet]
        [Route("downloadDocument")]
        public HttpResponseMessage DownloadDocument(string guid, string ext)
        {
            ext = (ext.Contains(".")) ? ext : "." + ext;
            string filePath = comparisonService.CalculateResultFileName(guid, "", ext);
            if (!string.IsNullOrEmpty(filePath))
            {
                if (File.Exists(filePath))
                {
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    var fileStream = new FileStream(filePath, FileMode.Open);
                    response.Content = new StreamContent(fileStream);
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    response.Content.Headers.ContentDisposition.FileName = System.IO.Path.GetFileName(filePath);
                    return response;
                }
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        /// <summary>
        /// Download results
        /// </summary>
        /// <param name=""></param>
        [HttpGet]
        [Route("downloadDocument")]
        public HttpResponseMessage DownloadDocument(string guid, string ext, string index)
        {
            ext = (ext.Contains(".")) ? ext : "." + ext;
            string filePath = comparisonService.CalculateResultFileName(guid, index, ext);
            if (!string.IsNullOrEmpty(filePath))
            {
                if (File.Exists(filePath))
                {
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    var fileStream = new FileStream(filePath, FileMode.Open);
                    response.Content = new StreamContent(fileStream);
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    response.Content.Headers.ContentDisposition.FileName = System.IO.Path.GetFileName(filePath);
                    return response;
                }
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        /// <summary>
        /// Upload document
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        [Route("uploadDocument")]
        public HttpResponseMessage UploadDocument()
        {
            try
            {
                string url = HttpContext.Current.Request.Form["url"];
                // get documents storage path
                string documentStoragePath = globalConfiguration.Comparison.GetFilesDirectory();
                bool rewrite = bool.Parse(HttpContext.Current.Request.Form["rewrite"]);
                string fileSavePath = "";
                if (string.IsNullOrEmpty(url))
                {
                    if (HttpContext.Current.Request.Files.AllKeys != null)
                    {
                        // Get the uploaded document from the Files collection
                        var httpPostedFile = HttpContext.Current.Request.Files["file"];
                        if (httpPostedFile != null)
                        {
                            if (rewrite)
                            {
                                // Get the complete file path
                                fileSavePath = System.IO.Path.Combine(documentStoragePath, httpPostedFile.FileName);
                            }
                            else
                            {
                                fileSavePath = Resources.GetFreeFileName(documentStoragePath, httpPostedFile.FileName);
                            }

                            // Save the uploaded file to "UploadedFiles" folder
                            httpPostedFile.SaveAs(fileSavePath);
                        }
                    }
                }
                else
                {
                    using (WebClient client = new WebClient())
                    {
                        // get file name from the URL
                        Uri uri = new Uri(url);
                        string fileName = System.IO.Path.GetFileName(uri.LocalPath);
                        if (rewrite)
                        {
                            // Get the complete file path
                            fileSavePath = System.IO.Path.Combine(documentStoragePath, fileName);
                        }
                        else
                        {
                            fileSavePath = Resources.GetFreeFileName(documentStoragePath, fileName);
                        }
                        // Download the Web resource and save it into the current filesystem folder.
                        client.DownloadFile(url, fileSavePath);
                    }
                }
                UploadedDocumentEntity uploadedDocument = new UploadedDocumentEntity();
                uploadedDocument.guid = fileSavePath;
                return Request.CreateResponse(HttpStatusCode.OK, uploadedDocument);
            }
            catch (System.Exception ex)
            {
                // set exception message
                return Request.CreateResponse(HttpStatusCode.OK, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Compare files from local storage
        /// </summary>
        /// <param name="compareRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("compareWithPaths")]
        public HttpResponseMessage CompareWithPaths(CompareRequest compareRequest)
        {
            try
            {
                // check formats
                if (comparisonService.CheckFiles(compareRequest.firstPath, compareRequest.secondPath))
                {
                    // compare
                    return Request.CreateResponse(HttpStatusCode.OK, comparisonService.Compare(compareRequest));
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new Resources().GenerateException(new Exception("Document types are different")));
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Compare documents from form formats
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        [Route("compareFiles")]
        public HttpResponseMessage CompareFiles()
        {
            try
            {
                CompareRequest requestData = comparisonService.GetFormData(HttpContext.Current.Request);
                // check formats
                if (comparisonService.CheckFiles(requestData.firstPath, requestData.secondPath))
                {
                    // compare
                    CompareResultResponse result = comparisonService.CompareFiles(requestData.files[0],
                        requestData.firstPassword,
                        requestData.files[1],
                        requestData.secondPassword,
                        System.IO.Path.GetExtension(requestData.firstPath));
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new Resources().GenerateException(new Exception("Document types are different")));
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Compare two files by urls
        /// </summary>
        /// <param name="compareRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("compareWithUrls")]
        public HttpResponseMessage CompareWithUrls(CompareRequest compareRequest)
        {
            try
            {
                string firstPath = compareRequest.firstPath;
                string secondPath = compareRequest.secondPath;
                // check formats
                if (comparisonService.CheckFiles(firstPath, secondPath))
                {
                    string firstPassword = compareRequest.firstPassword;
                    string secondPassword = compareRequest.secondPassword;
                    // open streams for urls
                    Stream firstContent = null;
                    Stream secondContent = null;
                    using (WebClient client = new WebClient())
                    {
                        byte[] firstFile = client.DownloadData(firstPath);
                        firstContent = new MemoryStream(firstFile);
                        byte[] secondFile = client.DownloadData(secondPath);
                        secondContent = new MemoryStream(secondFile);
                    };
                    // compare
                    CompareResultResponse compare = comparisonService.CompareFiles(firstContent, firstPassword, secondContent, secondPassword, System.IO.Path.GetExtension(firstPath));
                    return Request.CreateResponse(HttpStatusCode.OK, compare);
                }
                else
                {
                    throw new Exception("Document types are different");
                }
            }
            catch (IOException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new Resources().GenerateException(new Exception("Exception occurred while compare files by urls.")));
            }
        }

        /// <summary>
        /// Get result page
        /// </summary>
        /// <param name="loadResultPageRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("loadResultPage")]
        public HttpResponseMessage LoadResultPage(PostedDataEntity loadResultPageRequest)
        {
            return Request.CreateResponse(HttpStatusCode.OK, comparisonService.LoadResultPage(loadResultPageRequest));
        }

        /// <summary>
        ///  Compare 2 files got by different ways
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        [Route("compare")]
        public HttpResponseMessage Compare()
        {
            try
            {
                CompareRequest requestData = comparisonService.GetFormData(HttpContext.Current.Request);
                // transform all files into input streams
                TransformFiles transformFiles = new TransformFiles(requestData.files, requestData.passwords, requestData.urls, requestData.paths).TransformToStreams();
                List<string> fileNames = transformFiles.GetFileNames();

                // check formats
                if (comparisonService.CheckMultiFiles(fileNames))
                {
                    // get file extension
                    string ext = System.IO.Path.GetExtension(fileNames[0]);

                    // compare
                    List<Stream> newFiles = transformFiles.GetNewFiles();
                    List<string> newPasswords = transformFiles.GetNewPasswords();                  
                    return Request.CreateResponse(HttpStatusCode.OK, comparisonService.CompareFiles(newFiles[0], newPasswords[0], newFiles[1], newPasswords[1], ext));
                }
                else
                {
                    throw new Exception("Document types are different");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Compare several files got by different ways
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        [Route("multiCompare")]
        public HttpResponseMessage MultiCompare()
        {
            try
            {
                CompareRequest requestData = comparisonService.GetFormData(HttpContext.Current.Request);
                // transform all files into input streams
                TransformFiles transformFiles = new TransformFiles(requestData.files, requestData.passwords, requestData.urls, requestData.paths).TransformToStreams();
                List<string> fileNames = transformFiles.GetFileNames();

                // check formats
                if (comparisonService.CheckMultiFiles(fileNames))
                {
                    // get file extension
                    string ext = System.IO.Path.GetExtension(fileNames[0]);

                    // compare
                    List<Stream> newFiles = transformFiles.GetNewFiles();
                    List<string> newPasswords = transformFiles.GetNewPasswords();
                    return Request.CreateResponse(HttpStatusCode.OK, comparisonService.MultiCompareFiles(newFiles, newPasswords, ext));
                }
                else
                {
                    throw new Exception("Document types are different");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new Resources().GenerateException(ex));
            }
        }
    }
}