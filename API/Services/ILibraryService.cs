using API.DTOs;

namespace API.Services;
public interface ILibraryService : IServiceBase
{
    List<LibraryDTO> GetUserLibrariesById();
    LibraryDTO GetUserLibraryById();
    List<UploadDTO> GetLibraryUploadsById();
    UploadDTO GetLibraryUploadById();
}