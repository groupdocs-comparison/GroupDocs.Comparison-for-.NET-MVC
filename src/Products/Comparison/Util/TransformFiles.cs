
using GroupDocs.Comparison.MVC.Products.Comparison.Model.Request;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;

namespace GroupDocs.Comparison.MVC.Products.Comparison.Util
{
    public class TransformFiles
    {
        public List<Stream> files;
        public List<string> passwords;
        public List<CompareFileDataRequest> urls;
        public List<CompareFileDataRequest> paths;
        public List<Stream> newFiles;
        public List<string> fileNames;
        public List<string> newPasswords;

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
                    fileNames.Add(url.file);
                    using (WebClient client = new WebClient())
                    {
                        byte[] file = client.DownloadData(url.file);
                        newFiles.Add(new MemoryStream(file));
                    };
                    newPasswords.Add(url.password);
                }
            }
            if (paths != null)
            {
                // transform paths
                foreach (CompareFileDataRequest pathRequest in paths)
                {
                    fileNames.Add(pathRequest.file);
                    MemoryStream file = new MemoryStream();
                    using (Stream stream = File.Open(pathRequest.file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        stream.CopyTo(file);
                        file.Position = 0;
                        newFiles.Add(file);
                    }
                    newPasswords.Add(pathRequest.password);
                }
            }
            return this;
        }
    }
}