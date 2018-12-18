using System.Collections.Generic;

namespace GroupDocs.Comparison.MVC.Products.Common.Entity.Web
{
    public class LoadDocumentEntity
    {
        ///Document Guid
        public string guid;
        
        ///list of pages        
        public List<PageDescriptionEntity> pages;
    }
}