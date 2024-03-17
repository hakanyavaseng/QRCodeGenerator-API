using Microsoft.AspNetCore.Mvc;
using QRCodeGenerator.API.Concretes;
using System.Threading.Tasks;

namespace QRCodeGenerator.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QrCodeGeneratorController : ControllerBase
    {
        private readonly MessageService messageService;

        public QrCodeGeneratorController()
        {
            messageService = new MessageService();
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return BadRequest("Data is required");
            }

            byte[] responseQr = await messageService.SendMessage(data);

            // Return the QR code image as a file
            return File(responseQr, "image/png");
        }
    }
}
