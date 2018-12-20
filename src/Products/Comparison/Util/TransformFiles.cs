using GroupDocs.Comparison.MVC.Products.Comparison.Model.Request;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace GroupDocs.Comparison.MVC.Products.Comparison.Util
{
    public class TransformFiles
    {
        private readonly List<Stream> files;
        private readonly List<string> passwords;
        private readonly List<CompareFileDataRequest> urls;
        private readonly List<CompareFileDataRequest> paths;
        private List<Stream> newFiles;
        private List<string> fileNames;
        private List<string> newPasswords;

        public List<Stream> GetNewFiles() {
            return newFiles;
        }

        public List<string> GetFileNames()
        {
            return fileNames;
        }

        public List<string> GetNewPasswords()
        {
            return newPasswords;
        }

        public TransformFiles(List<Stream> files, List<string> passwords, List<CompareFileDataRequest> urls, List<CompareFileDataRequest> paths)
        {
            this.files = files;
            this.passwords = passwords;
            this.urls = urls;
            this.paths = paths;
        }

        public TransformFiles TransformToStreams()
        {
            newFiles = new List<Stream>();
            fileNames = new List<string>();
            newPasswords = new List<string>();
            if (files.Count > 0)
            {
                for(int i = 0; i < files.Count; i++)
                {
                    files[i].Position = 0;
                    newFiles.Add(files[i]);
                    string password = (passwords == null) ? "" : passwords[i];
                    newPasswords.Add(password);
                }
            }
            // transform urls
            if (urls != null)
            {
                foreach (CompareFileDataRequest url in urls)
                {
                    fileNames.Add(url.GetFile());
                    using (WebClient client = new WebClient())
                    {
                        byte[] file = client.DownloadData(url.GetFile());
                        newFiles.Add(new MemoryStream(file));
                    };
                    newPasswords.Add(url.GetPassword());
                }
            }
            if (paths != null)
            {
                // transform paths
                foreach (CompareFileDataRequest pathRequest in paths)
                {
                    fileNames.Add(pathRequest.GetFile());
                    MemoryStream file = new MemoryStream();
                    using (Stream stream = File.Open(pathRequest.GetFile(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        stream.CopyTo(file);
                        file.Position = 0;
                        newFiles.Add(file);
                    }
                    newPasswords.Add(pathRequest.GetPassword());
                }
            }
            return this;
        }
    }
}