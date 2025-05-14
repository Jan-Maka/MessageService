using System.ComponentModel.DataAnnotations;

namespace Project.Server.Models
{
    public class Attachment
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? MessageId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        public string ContentType { get; set; }

        public string FileHash { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    }
}
