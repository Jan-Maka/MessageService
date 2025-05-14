using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.Server.Data;
using Project.Server.DTOs;
using Project.Server.Models;
using Project.Server.Service;

namespace Project.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMessageService _messageService;
        private readonly IAttachmentService _attachmentService;
        public MessageController(IUserService userService, IMessageService messageService, IAttachmentService attachmentService)
        {
            _userService = userService;
            _messageService = messageService;
            _attachmentService = attachmentService;
        }

        [Authorize]
        [HttpPost("create/attachments")]
        public async Task<ActionResult<List<AttachmentDTO>>> AttachFilesToMessages(List<IFormFile> files)
        {
            User user = _userService.GetLoggedInUser(User);
            if (user == null) return Unauthorized();

            if (files.Count() == 0) return BadRequest();

            List<AttachmentDTO> attachments = await _attachmentService.AttachFilesToMessage(files, user.Id);

            return Ok(attachments);
        }

        [Authorize]
        [HttpGet("download/attachment")]
        public IActionResult DownloadMessageAttachment([FromQuery(Name = "messageId")] int messageId, [FromQuery(Name = "attachmentId")] int attachmentId)
        {
            User user = _userService.GetLoggedInUser(User);
            if (user == null) return Unauthorized();

            var message = _messageService.GetMessageById(messageId);
            if (message == null) return BadRequest();

            var attachment = _attachmentService.GetAttachmentById(attachmentId);
            if (attachment == null) return BadRequest();

            if (attachment.MessageId != messageId) return Unauthorized();

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), $"Uploads/{message.SenderId}", Path.GetFileName(attachment.FilePath));
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found on the server.");
            }

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(stream, attachment.ContentType, attachment.FileName);
        }

        [Authorize]
        [HttpGet("view/attachment")]
        public IActionResult ViewFile([FromQuery (Name ="messageId")] int messageId,[FromQuery(Name = "attachmentId")] int attachmentId) {
            User user = _userService.GetLoggedInUser(User);
            if (user == null) return Unauthorized();

            var message = _messageService.GetMessageById(messageId);
            if (message == null) return BadRequest();

            var attachment = _attachmentService.GetAttachmentById(attachmentId);
            if(attachment == null) return BadRequest();

            if (attachment.MessageId != messageId) return Unauthorized();
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), $"Uploads/{message.SenderId}", Path.GetFileName(attachment.FilePath));
            return PhysicalFile(filePath, attachment.ContentType);

        }

    }
}
