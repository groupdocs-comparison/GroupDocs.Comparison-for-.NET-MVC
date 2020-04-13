using System;
using System.Collections.Generic;
using System.IO;
using GroupDocs.Comparison.MVC.Products.Common.Entity.Web;
using GroupDocs.Comparison.MVC.Products.Comparison.Model.Response;
using GroupDocs.Comparison.MVC.Products.Common.Config;
using GroupDocs.Comparison.MVC.Products.Common.Util.Comparator;
using GroupDocs.Comparison.MVC.Products.Comparison.Model.Request;
using GroupDocs.Comparison.Result;
using GroupDocs.Comparison.Interfaces;
using GroupDocs.Comparison.Options;

namespace GroupDocs.Comparison.MVC.Products.Comparison.Service
{
    public class ComparisonServiceImpl : IComparisonService
    {
        private readonly GlobalConfiguration globalConfiguration;

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
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(file);
                    // check if current file/folder is hidden
                    if (!(fileInfo.Attributes.HasFlag(FileAttributes.Hidden) ||
                        Path.GetFileName(file).Equals(Path.GetFileName(globalConfiguration.Comparison.GetFilesDirectory()))))
                    {
                        FileDescriptionEntity fileDescription = new FileDescriptionEntity
                        {
                            guid = Path.GetFullPath(file),
                            name = Path.GetFileName(file),
                            // set is directory true/false
                            isDirectory = fileInfo.Attributes.HasFlag(FileAttributes.Directory)
                        };
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
                throw new FileLoadException("Exception occurred while loading files", ex);
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

        public LoadDocumentEntity LoadDocumentPages(string path, string password, bool loadAllPages)
        {
            LoadDocumentEntity loadDocumentEntity = new LoadDocumentEntity();
            //load file with results
            try
            {
                using (Comparer comparer = new Comparer(path))
                {
                    List<string> pagesContent = new List<string>();
                    IDocumentInfo documentInfo = comparer.Source.GetDocumentInfo();

                    if (loadAllPages)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(path);

                        PreviewOptions previewOptions = new PreviewOptions(pageNumber =>
                        {
                            var pagePath = Path.Combine(globalConfiguration.Comparison.GetFilesDirectory(), $"{fileName}_{pageNumber}.png");
                            return File.Create(pagePath);
                        })
                        {
                            PreviewFormat = PreviewFormats.PNG,
                            PageNumbers = GetPagesNumbersArray(documentInfo.PageCount)
                        };
                        comparer.Source.GeneratePreview(previewOptions);

                        string[] files = Directory.GetFiles(globalConfiguration.Comparison.GetFilesDirectory(),
                                fileName + "_*" + ".png");

                        foreach (string file in files)
                        {
                            using (FileStream outputStream = File.OpenRead(Path.Combine(file)))
                            {
                                var memoryStream = new MemoryStream();
                                outputStream.CopyTo(memoryStream);

                                byte[] pageBytes = memoryStream.ToArray();
                                string encodedImage = Convert.ToBase64String(pageBytes);

                                pagesContent.Add(encodedImage);
                            }
                        }
                    }

                    for (int i = 0; i < documentInfo.PageCount; i++)
                    {
                        PageDescriptionEntity pageData = new PageDescriptionEntity
                        {
                            height = 842,
                            width = 595,
                            number = i + 1
                        };

                        if (pagesContent.Count > 0)
                        {
                            pageData.SetData(pagesContent[i]);
                        }

                        loadDocumentEntity.SetPages(pageData);
                    }

                    return loadDocumentEntity;
                }
            }
            catch (Exception ex)
            {
                throw new FileLoadException("Exception occurred while loading result pages", ex);
            }
        }

        static int[] GetPagesNumbersArray(int arrLength)
        {
            int[] pageNumbersArr = new int[arrLength];

            for (int i = 0; i < arrLength; i++)
            {
                pageNumbersArr[i] = i + 1;
            }

            return pageNumbersArr;
        }

        public PageDescriptionEntity LoadDocumentPage(PostedDataEntity postedData)
        {
            PageDescriptionEntity loadedPage = new PageDescriptionEntity();

            try
            {
                // get/set parameters
                string documentGuid = postedData.guid;
                int pageNumber = postedData.page;
                string password = (string.IsNullOrEmpty(postedData.password)) ? null : postedData.password;

                using (FileStream fileStream = File.Open(documentGuid, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    using (Comparer comparer = new Comparer(fileStream, GetLoadOptions(password)))
                    {

                        byte[] bytes = RenderPageToMemoryStream(comparer, pageNumber - 1).ToArray();
                        string encodedImage = Convert.ToBase64String(bytes);
                        loadedPage.SetData(encodedImage);

                        loadedPage.number = pageNumber;
                        loadedPage.height = 842;
                        loadedPage.width = 595;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FileLoadException("Exception occurred while loading result page", ex);
            }

            return loadedPage;
        }

        static MemoryStream RenderPageToMemoryStream(Comparer comparer, int pageNumberToRender)
        {
            MemoryStream result = new MemoryStream();

            PreviewOptions previewOptions = new PreviewOptions(pageNumber => result)
            {
                PreviewFormat = PreviewFormats.PNG,
                PageNumbers = new[] { pageNumberToRender },
                ReleasePageStream = UserReleaseStreamMethod
            };

            comparer.Source.GeneratePreview(previewOptions);

            return result;
        }

        private static void UserReleaseStreamMethod(int pageNumber, Stream stream)
        {
            stream.Close();
        }

        private static LoadOptions GetLoadOptions(string password)
        {
            LoadOptions loadOptions = new LoadOptions
            {
                Password = password
            };

            return loadOptions;
        }

        private CompareResultResponse CompareTwoDocuments(CompareRequest compareRequest)
        {
            // to get correct coordinates we will compare document twice
            // this is a first comparing to get correct coordinates of the insertions and style changes
            string extension = Path.GetExtension(compareRequest.guids[0].GetGuid());
            string guid = System.Guid.NewGuid().ToString();
            //save all results in file
            string resultGuid = Path.Combine(globalConfiguration.Comparison.GetResultDirectory(), guid + extension);

            Comparer compareResult = CompareFiles(compareRequest, resultGuid);
            ChangeInfo[] changes = compareResult.GetChanges();

            CompareResultResponse compareResultResponse = GetCompareResultResponse(changes, resultGuid);
            compareResultResponse.SetExtension(extension);
            return compareResultResponse;
        }

        private Comparer CompareFiles(CompareRequest compareRequest, string resultGuid)
        {
            string firstPath = compareRequest.guids[0].GetGuid();
            string secondPath = compareRequest.guids[1].GetGuid();

            // create new comparer
            using (Comparer comparer = new Comparer(firstPath))
            {
                comparer.Add(secondPath);
                CompareOptions compareOptions = new CompareOptions() { CalculateCoordinates = true };
                using (FileStream outputStream = File.Create(Path.Combine(resultGuid)))
                {
                    comparer.Compare(outputStream, compareOptions);
                }

                return comparer;
            }
        }

        private CompareResultResponse GetCompareResultResponse(ChangeInfo[] changes, string resultGuid)
        {
            CompareResultResponse compareResultResponse = new CompareResultResponse();
            compareResultResponse.SetChanges(changes);

            List<PageDescriptionEntity> pages = LoadDocumentPages(resultGuid, "", true).GetPages();

            compareResultResponse.SetPages(pages);
            compareResultResponse.SetGuid(resultGuid);
            return compareResultResponse;
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
