using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using dashboard.ui.Models;
using Newtonsoft.Json;

namespace dashboard.ui.Controllers
{
    public class NotificationController : Controller
    {
        // GET: Notification
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetNotifications()
        {
            var result = new List<Notification>();
            string root = $@"C:\dev\Stores\notifications";

            DirectoryInfo rootInfo = new DirectoryInfo(root);

            if (rootInfo.Exists)
            {
                var files = rootInfo.GetFiles();

                foreach (var fileInfo in files)
                {
                    Notification notification = JsonConvert.DeserializeObject<Notification>(System.IO.File.ReadAllText(fileInfo.FullName));

                    result.Add(notification);
                }
            }

            return PartialView("_Notification", result);
        }
    }
}
