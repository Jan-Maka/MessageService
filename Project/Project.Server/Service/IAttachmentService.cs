using Project.Server.DTOs;
using Project.Server.Models;

namespace Project.Server.Service
{
    public interface IAttachmentService
    {
        Attachment GetAttachmentById(int id);
        AttachmentDTO CreateAttachmentDTO(Attachment attachment);

        List<AttachmentDTO> CreateAttachmentDTOList(List<Attachment> attachments);
        Task<List<AttachmentDTO>> AttachFilesToMessage(List<IFormFile> files, int userId);


    }
}
