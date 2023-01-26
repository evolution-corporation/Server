using Microsoft.AspNetCore.Mvc;
using IApplicationLifetime = Microsoft.AspNetCore.Hosting.IApplicationLifetime;

namespace Server.Controllers
{
    public class WebServicesController : Controller
    {
        [Obsolete("Obsolete")] private IApplicationLifetime ApplicationLifetime { get; set; }

        public WebServicesController(IApplicationLifetime appLifetime)
        {
            ApplicationLifetime = appLifetime;
        }

        public void ShutdownSite()
        {
            ApplicationLifetime.StopApplication();
        }
    }
}