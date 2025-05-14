
using Microsoft.EntityFrameworkCore;
using Project.Server.Data;
using Project.Server.DTOs;
using Project.Server.Models;
using System.Security.Cryptography;

namespace Project.Server.Service
{
    public class AttachmentService : IAttachmentService
    {
        private readonly ApplicationDbContext _db;
        public AttachmentService(ApplicationDbContext db)
        {
            _db = db;
        }

        private string ComputeFileHash(IFormFile file)
        {
            using (var sha256 = SHA256.Create())
            using (var stream = file.OpenReadStream())
            {
                byte[] hash = sha256.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        public AttachmentDTO CreateAttachmentDTO(Attachment attachment)
        {
            return new AttachmentDTO(attachment.Id, attachment.FileName, attachment.ContentType, attachment.UploadedAt, attachment.FilePath);
        }

        public List<AttachmentDTO> CreateAttachmentDTOList(List<Attachment> attachmnets) {
            return attachmnets.Select(a => CreateAttachmentDTO(a)).ToList();
        }


        public async Task<List<AttachmentDTO>> AttachFilesToMessage(List<IFormFile> files, int userId)
        {
            List<AttachmentDTO> attachments = new List<AttachmentDTO>();
            foreach (var file in files)
            {

                string fileHash = ComputeFileHash(file);
                var existingAttachment = _db.Attachments.FirstOrDefault(a => a.UserId == userId && a.FileHash == fileHash);
                if (existingAttachment != null)
                {
                    Attachment attachment = new Attachment
                    {
                        UserId = userId,
                        FileName = file.FileName,
                        FilePath = existingAttachment.FilePath,
                        ContentType = existingAttachment.ContentType,
                        FileHash = existingAttachment.FileHash,
                    };
                    _db.Attachments.Add(attachment);
                    await _db.SaveChangesAsync();
                    attachments.Add(CreateAttachmentDTO(attachment));
                }
                else
                {
                    string userDir = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", userId.ToString());
                    string uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                    string filePath = Path.Combine(userDir, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    Attachment attachment = new Attachment
                    {
                        UserId = userId,
                        FileName = file.FileName,
                        FilePath = $"/uploads/{userId}/{uniqueFileName}",
                        ContentType = file.ContentType,
                        FileHash = fileHash,
                    };

                    _db.Attachments.Add(attachment);
                    await _db.SaveChangesAsync();
                    attachments.Add(CreateAttachmentDTO(attachment));
                }

            }
            return attachments;
        }

        public Attachment GetAttachmentById(int id)
        {
            return _db.Attachments.FirstOrDefault(a => a.Id == id);
        }
    }
}
