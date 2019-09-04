using System;
using System.Collections.Generic;
using System.IO;
using GroupDocs.Comparison.MVC.Products.Common.Entity.Web;
using GroupDocs.Comparison.MVC.Products.Comparison.Model.Response;
using GroupDocs.Comparison.MVC.Products.Common.Config;
using GroupDocs.Comparison.MVC.Products.Common.Util.Comparator;
using GroupDocs.Comparison.MVC.Products.Comparison.Model.Request;
using GroupDocs.Comparison;
using GroupDocs.Comparison.Options;
using GroupDocs.Comparison.Changes;
using System.Linq;

namespace GroupDocs.Comparison.MVC.Products.Comparison.Service
{
    public class ComparisonServiceImpl : IComparisonService
    {
        private static readonly string DOCX = ".docx";
        private static readonly string DOC = ".doc";
        private static readonly string XLS = ".xls";
        private static readonly string XLSX = ".xlsx";
        private static readonly string PPT = ".ppt";
        private static readonly string PPTX = ".pptx";
        private static readonly string PDF = ".pdf";
        private static readonly string TXT = ".txt";
        private static readonly string HTML = ".html";
        private static readonly string HTM = ".htm";

        private GlobalConfiguration globalConfiguration;

        public ComparisonServiceImpl(GlobalConfiguration globalConfiguration)
        {
            this.globalConfiguration = globalConfiguration;
        }

        public List<FileDescriptionEntity> LoadFiles(PostedDataEntity fileTreeRequest)
        {
            // get request body       
            string relDirPath = fileTreeRequest.path;
            // get file list from storage path
            try
            {
                // get all the files from a directory
                if (string.IsNullOrEmpty(relDirPath))
                {
                    relDirPath = globalConfiguration.Comparison.GetFilesDirectory();
                }
                else
                {
                    relDirPath = Path.Combine(globalConfiguration.Comparison.GetFilesDirectory(), relDirPath);
                }

                List<string> allFiles = new List<string>(Directory.GetFiles(relDirPath));
                allFiles.AddRange(Directory.GetDirectories(relDirPath));
                List<FileDescriptionEntity> fileList = new List<FileDescriptionEntity>();

                allFiles.Sort(new FileNameComparator());
                allFiles.Sort(new FileTypeComparator());

                foreach (string file in allFiles)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    // check if current file/folder is hidden
                    if (fileInfo.Attributes.HasFlag(FileAttributes.Hidden) ||
                        Path.GetFileName(file).Equals(Path.GetFileName(globalConfiguration.Comparison.GetFilesDirectory())))
                    {
                        // ignore current file and skip to next one
                        continue;
                    }
                    else
                    {
                        FileDescriptionEntity fileDescription = new FileDescriptionEntity();
                        fileDescription.guid = Path.GetFullPath(file);
                        fileDescription.name = Path.GetFileName(file);
                        // set is directory true/false
                        fileDescription.isDirectory = fileInfo.Attributes.HasFlag(FileAttributes.Directory);
                        // set file size
                        if (!fileDescription.isDirectory)
                        {
                            fileDescription.size = fileInfo.Length;
                        }
                        // add object to array list
                        fileList.Add(fileDescription);
                    }
                }
                return fileList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CheckFiles(CompareRequest files)
        {
            string extension = Path.GetExtension(files.guids[0].GetGuid());
            // check if files extensions are the same and support format file
            if (!CheckSupportedFiles(extension))
            {
                return false;
            }
            foreach (CompareFileDataRequest path in files.guids)
            {
                if (!extension.Equals(Path.GetExtension(path.GetGuid())))
                {
                    return false;
                }
            }
            return true;
        }

        public CompareResultResponse Compare(CompareRequest compareRequest)
        {            
            CompareResultResponse compareResultResponse = CompareTwoDocuments(compareRequest);
            return compareResultResponse;
        }

        public LoadDocumentEntity LoadDocumentPages(string path, string password)
        {
            LoadDocumentEntity loadDocumentEntity = new LoadDocumentEntity();
            //load file with results
            try
            {
                Comparer comparer = new Comparer();
                List<PageImage> resultImages = comparer.ConvertToImages(path, password);

                foreach (PageImage page in resultImages)
                {
                    PageDescriptionEntity loadedPage = new PageDescriptionEntity();
                    byte[] bytes = null;
                    page.PageStream.Position = 0;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        page.PageStream.CopyTo(ms);
                        bytes = ms.ToArray();
                    }
                    loadedPage.SetData(Convert.ToBase64String(bytes));
                    loadedPage.height = page.Height;
                    loadedPage.width = page.Width;
                    loadedPage.number = page.PageNumber;
                    loadDocumentEntity.SetPages(loadedPage);
                }
                return loadDocumentEntity;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception occurred while loading result page", ex);
            }
        }

        public PageDescriptionEntity LoadDocumentPage(PostedDataEntity postedData)
        {
            PageDescriptionEntity loadedPage = new PageDescriptionEntity();

            string password = "";
            try
            {
                // get/set parameters
                string documentGuid = postedData.guid;
                int pageNumber = postedData.page;
                password = (String.IsNullOrEmpty(postedData.password)) ? null : postedData.password;
                Comparer comparer = new Comparer();
                List<PageImage> resultImages = comparer.ConvertToImages(documentGuid, password);

                byte[] bytes = null;
                resultImages[pageNumber - 1].PageStream.Position = 0;
                using (MemoryStream ms = new MemoryStream())
                {
                    resultImages[pageNumber - 1].PageStream.CopyTo(ms);
                    bytes = ms.ToArray();
                }
                loadedPage.SetData(Convert.ToBase64String(bytes));
                loadedPage.number = pageNumber;
                loadedPage.height = resultImages[pageNumber - 1].Height;
                loadedPage.width = resultImages[pageNumber - 1].Width;

            }
            catch (System.Exception ex)
            {
                // set exception message
                throw new Exception("Exception occurred while loading result page", ex);
            }
            return loadedPage;
        }

        public LoadDocumentEntity LoadDocumentInfo(PostedDataEntity postedData)
        {
            LoadDocumentEntity loadDocumentEntity = new LoadDocumentEntity();

            string password = "";
            try
            {
                // get/set parameters
                string documentGuid = postedData.guid;
                password = (String.IsNullOrEmpty(postedData.password)) ? null : postedData.password;
                Comparer comparer = new Comparer();
                List<PageImage> resultImages = comparer.ConvertToImages(documentGuid, password);

                foreach (PageImage page in resultImages)
                {
                    PageDescriptionEntity loadedPage = new PageDescriptionEntity();
                    loadDocumentEntity.SetPages(loadedPage);
                }
                return loadDocumentEntity;
            }
            catch (System.Exception ex)
            {
                // set exception message
                throw new FileLoadException("Exception occurred while loading document info", ex);
            }
        }



        private CompareResultResponse CompareTwoDocuments(CompareRequest compareRequest)
        {
            // to get correct coordinates we will compare document twice
            // this is a first comparing to get correct coordinates of the insertions and style changes
            ICompareResult compareResult = CompareFiles(compareRequest);
            string extension = Path.GetExtension(compareRequest.guids[0].GetGuid());
            ChangeInfo[] changes = compareResult.GetChanges();
            CompareResultResponse compareResultResponse = GetCompareResultResponse(compareResult, changes, extension);
            compareResultResponse.SetExtension(extension);
            return compareResultResponse;
        }

        private ICompareResult CompareFiles(CompareRequest compareRequest)
        {
            string firstPath = compareRequest.guids[0].GetGuid();
            ICompareResult compareResult;
            // create new comparer
            Comparer comparer = new Comparer();
            // create setting for comparing
            ComparisonSettings settings = new ComparisonSettings();
            settings.StyleChangeDetection = true;
            settings.CalculateComponentCoordinates = true;

            compareResult = comparer.Compare(firstPath,
                compareRequest.guids[0].GetPassword(),
                compareRequest.guids[1].GetGuid(),
                compareRequest.guids[1].GetPassword(),
                settings);

            if (compareResult == null)
            {
                throw new Exception("Something went wrong. We've got null result.");
            }
            return compareResult;
        }

        private CompareResultResponse GetCompareResultResponse(ICompareResult compareResult, ChangeInfo[] changes, string ext)
        {
            CompareResultResponse compareResultResponse = new CompareResultResponse();
            compareResultResponse.SetChanges(changes);

            string guid = System.Guid.NewGuid().ToString();
            compareResultResponse.SetGuid(guid);
            //save all results in file
            string resultGuid = SaveFile(compareResultResponse.GetGuid(), compareResult.GetStream(), ext);
            List<PageDescriptionEntity> pages = LoadDocumentPages(resultGuid, "").GetPages();

            compareResultResponse.SetPages(pages);
            compareResultResponse.SetGuid(resultGuid);
            return compareResultResponse;
        }

        private string SaveFile(string guid, Stream inputStream, string ext)
        {
            string fileName = Path.Combine(globalConfiguration.Comparison.GetResultDirectory(), guid + ext);
            try
            {
                using (var fileStream = File.Create(fileName))
                {
                    inputStream.Seek(0, SeekOrigin.Begin);
                    inputStream.CopyTo(fileStream);
                    inputStream.Close();
                }
            }
            catch (IOException)
            {
                throw new Exception("Exception occurred while write result images files.");
            }
            return fileName;
        }

        /// <summary>
        /// Fix file extensions for some formats
        /// </summary>
        /// <param name="ext">string</param>
        /// <returns></returns>
        private string GetRightExt(string ext)
        {
            switch (ext)
            {
                case ".doc":
                    return DOC;
                case ".docx":
                    return DOCX;
                case ".xls":
                    return XLS;
                case ".xlsx":
                    return XLSX;
                case ".ppt":
                    return PPT;
                case ".pptx":
                    return PPTX;
                case ".pdf":
                    return PDF;
                case ".txt":
                    return TXT;
                case ".html":
                    return HTML;
                case ".htm":
                    return HTM;
                default:
                    return ext;
            }
        }

        /// <summary>
        /// Check support formats for comparing
        /// </summary>
        /// <param name="extension"></param>
        /// <returns>string</returns>
        private bool CheckSupportedFiles(string extension)
        {
            switch (extension)
            {
                case ".doc":
                case ".docx":
                case ".xls":
                case ".xlsx":
                case ".ppt":
                case ".pptx":
                case ".pdf":
                case ".txt":
                case ".html":
                case ".htm":
                    return true;
                default:
                    return false;
            }
        }
    }
}
