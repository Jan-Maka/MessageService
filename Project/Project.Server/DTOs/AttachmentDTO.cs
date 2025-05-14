namespace Project.Server.DTOs
{
    public class AttachmentDTO
    {
        public int Id { get; set; }
        public string FileName { get; set; }

        public string ContentType { get; set; }

        public DateTime UploadedAt { get; set; }

        public string FilePath { get; set; }

        public AttachmentDTO()
        {
           
        }

        public AttachmentDTO(int id, string fileName, string contentType, DateTime uploadedAt, string filePath)
        {
            Id = id;
            FileName = fileName;
            ContentType = contentType;
            UploadedAt = uploadedAt;
            FilePath = filePath;
        }
    }

}
