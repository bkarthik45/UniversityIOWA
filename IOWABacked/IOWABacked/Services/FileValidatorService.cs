using System;
using System.Net.Mime;
using IOWABacked.Services.Interfaces;

namespace IOWABacked.Services
{
	public class FileValidatorService: IFileValidatorService
    {
        private readonly string[] _allowedExtensions = { ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx" };
        private const long _maxFileSize = 5 * 1024 * 1024; 

        public bool IsValid(IFormFile file, out string errorMessage)
        {
            errorMessage = "";

            if (file == null || file.Length == 0)
            {
                errorMessage = "File is required.";
                return false;
            }

            var ext = Path.GetExtension(file.FileName).ToLower();
                var contentType = file.ContentType.ToLower();

            if (!_allowedExtensions.Contains(ext))
            {
                errorMessage = "Only PDF, JPG, JPEG, PNG, DOC, and DOCX files are allowed.";
                return false;
            }
            if (!(contentType.StartsWith("image/") || contentType.Contains("pdf") || contentType.Contains("msword") || contentType.Contains("officedocument")))
            {
                errorMessage = "Invalid file type based on MIME.";
                return false;
            }
            if (file.Length > _maxFileSize)
            {
                errorMessage = "File size cannot exceed 5MB.";
                return false;
            }
            return true;
        }
    }
}

