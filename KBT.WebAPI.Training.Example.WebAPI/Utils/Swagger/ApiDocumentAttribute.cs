using System;
namespace KBT.WebAPI.Training.Example.WebAPI.Utils.Swagger
{
    public class ApiDocumentAttribute : Attribute
    {
        public string Version { get; set; }
        public string TitleName { get; set; }
        public string DocumentName { get; set; }
        public bool Authorize { get; set; }

        public ApiDocumentAttribute(string documentName = null, string titleName = null,
            bool authorize = false, string version = "v1")
        {
            Version = version;
            TitleName = titleName;
            DocumentName = documentName;
            Authorize = authorize;
        }
    }
}

