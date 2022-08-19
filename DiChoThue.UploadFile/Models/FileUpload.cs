using Microsoft.AspNetCore.Http;

namespace DiChoThue.UploadFile.Models
{
    public class FileUpload
    {
        public IFormFile files { get; set; }
    }
}
