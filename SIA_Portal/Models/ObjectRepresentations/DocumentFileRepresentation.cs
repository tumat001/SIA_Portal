using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIA_Portal.Models.ObjectRepresentations
{
    public class DocumentFileRepresentation
    {

        public int Id { set; get; }
        
        public string DirectoryPath { set; get; }

        public byte[] DocuBytes { set; get; }
        
        public string DocuExt { set; get; }

        public string OriginalFileName { set; get; }



        //

        public string GetMimeMappingOfDocu()
        {
            return MimeMapping.GetMimeMapping(OriginalFileName + DocuExt);
        }

    }
}