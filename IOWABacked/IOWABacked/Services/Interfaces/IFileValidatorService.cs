using Microsoft.AspNetCore.Http;

namespace IOWABacked.Services.Interfaces
{
    public interface IFileValidatorService
    {
        bool IsValid(IFormFile file, out string errorMessage);
    }
}
