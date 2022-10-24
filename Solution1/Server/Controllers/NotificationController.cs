// using Microsoft.AspNetCore.Mvc;
// using Server.Entities;
// using Server.Helpers;
// using Server.Services;
//
// namespace Server.Controllers;
// [ApiController]
// [Route("/subnotification")]
// public class NotificationController: ControllerBase
// {
//     private INotificationService service;
//     public NotificationController(INotificationService service, Notificator notificator)
//     {
//         this.service = service;
//     }
//
//     [HttpPut]
//     public IActionResult SubUser(string expoToken, int frequency)
//     {
//         var token = HttpContext.Request.Headers.Authorization.ToString();
//         service.SubUserNotification(token,expoToken,frequency);
//         return Ok();
//     }
//
//     [HttpDelete]
//     public IActionResult UnsubUser(string token)
//     {
//         service.UnsubUserNotification(token);
//         return Ok();
//     }
//
//     [HttpGet]
//     public IActionResult FotStartCreatingNotification()
//     {
//         return Ok();
//     }
// }