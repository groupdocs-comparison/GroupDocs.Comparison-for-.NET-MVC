
using Newtonsoft.Json;

namespace GroupDocs.Comparison.MVC.Products.Comparison.Model.Request
{
    public class CompareFileDataRequest
    {
        [JsonProperty]
        private string file { get; set; }

        [JsonProperty]
        private string password { get; set; }

        public void SetFile(string file)
        {
            this.file = file;
        }

        public string GetFile()
        {
            return file;
        }

        public void SetPassword(string password)
        {
            this.password = password;
        }

        public string GetPassword()
        {
            return password;
        }
    }
}